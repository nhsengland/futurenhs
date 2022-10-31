using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;

namespace FileServer.Wopi
{
    /// <summary>
    /// Defines a service that is tasked with checking a given proof has indeed been signed by a WOPI Client whom this
    /// application is configured to trust, and thus requests issued by it are considered non-repudiable
    /// </summary>
    /// <remarks>https://wopi.readthedocs.io/en/latest/scenarios/proofkeys.html</remarks>
    public interface IWopiCryptoProofChecker
    {
        (bool isInvalid, bool refetchProofKeys) IsProofInvalid(HttpRequest httpRequest, IWopiProofKeysProvider wopiProofKeysProvider);
    }

    public interface IWopiProofKeysProvider
    {
        bool IsEmpty { get; }

        string? PublicKeyCspBlob { get; }
        string? OldPublicKeyCspBlob { get; }
    }

    public sealed class WopiCryptoProofChecker : IWopiCryptoProofChecker
    {
        private readonly ILogger<WopiCryptoProofChecker> _logger;

        public WopiCryptoProofChecker(ILogger<WopiCryptoProofChecker> logger)
        {
            _logger = logger;
        }

        (bool isInvalid, bool refetchProofKeys) IWopiCryptoProofChecker.IsProofInvalid(HttpRequest httpRequest, IWopiProofKeysProvider wopiProofKeysProvider)
        {
            if (httpRequest is null) throw new ArgumentNullException(nameof(httpRequest));
            if (wopiProofKeysProvider is null) throw new ArgumentNullException(nameof(wopiProofKeysProvider));
            if (wopiProofKeysProvider.IsEmpty) throw new ArgumentOutOfRangeException(nameof(wopiProofKeysProvider), "The proof keys provider cannot be EMPTY.  Please check it's state before calling this method");

            const bool PROOF_IS_INVALID = true;
            const bool PROOF_IS_VALID = false;

            // When running behind an appliction gateway the url that the wopi client issued a request against is not necessarily the same
            // on to which the gateway ultimately routes, therefore the httpRequest.GetEncodedUrl() may not be returning the url that the wopi
            // client used to sign the proof.  This is not an easy problem to overcome.  The best I can come up with for now, is for the gateway
            // to set the Location header to the original request url and for this application to use it as an override if it is present (won't be 
            // for local dev etc).  The gateway should probably be doing this anyway to assure redirects work properly so shouldn't be any 
            // unexpected overhead.   Only other option I can think of (if this proves impractical) is to derive the URL used by the client ourselves
            // and use that to verify the signature

            var locationHeader = httpRequest.Headers["Location"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(locationHeader) && !Uri.IsWellFormedUriString(locationHeader, UriKind.Absolute))
            {
                throw new UriFormatException($"The uri in the location header is not considered a well formed absolute url : '{locationHeader}'");
            }

            var encodedUrl = new Uri(locationHeader ?? httpRequest.GetEncodedUrl(), UriKind.Absolute);

            var accessToken = HttpUtility.ParseQueryString(encodedUrl.Query)["access_token"];

            if (string.IsNullOrWhiteSpace(accessToken)) return (PROOF_IS_INVALID, refetchProofKeys: false);

            var encodedAccessToken = HttpUtility.UrlEncode(accessToken);

            var encodedRequestUrl = locationHeader ?? httpRequest.GetEncodedUrl();

            var wopiHostUrl = encodedRequestUrl.ToUpperInvariant();

            var wopiTimestamp = httpRequest.Headers["X-WOPI-Timestamp"].SingleOrDefault();

            if (string.IsNullOrWhiteSpace(wopiTimestamp)) throw new InvalidOperationException("The X-WOPI-Timestamp header is missing from the request and the caller is required to present proof");

            var timestamp = Convert.ToInt64(wopiTimestamp.Trim());

            var accessTokenBytes = Encoding.UTF8.GetBytes(encodedAccessToken);
            var wopiHostUrlBytes = Encoding.UTF8.GetBytes(wopiHostUrl);
            var timestampBytes = BitConverter.GetBytes(timestamp).Reverse().ToArray();

            var proof = new List<byte>(4 + accessTokenBytes.Length + 4 + wopiHostUrlBytes.Length + 4 + timestampBytes.Length);

            proof.AddRange(BitConverter.GetBytes(accessTokenBytes.Length).Reverse());
            proof.AddRange(accessTokenBytes);
            proof.AddRange(BitConverter.GetBytes(wopiHostUrlBytes.Length).Reverse());
            proof.AddRange(wopiHostUrlBytes);
            proof.AddRange(BitConverter.GetBytes(timestampBytes.Length).Reverse());
            proof.AddRange(timestampBytes);

            var expectedProof = proof.ToArray();

            var givenProof = httpRequest.Headers["X-WOPI-Proof"].Single().Trim();
            var oldGivenProof = httpRequest.Headers["X-WOPI-ProofOld"].Single().Trim();

            var publicKeyCspBlob = wopiProofKeysProvider.PublicKeyCspBlob;
            var oldPublicKeyCspBlob = wopiProofKeysProvider.OldPublicKeyCspBlob;

            _logger?.LogDebug("location-header = {LocationHeader}",         locationHeader ?? "null");
            _logger?.LogDebug("request_url = {EncodedRequestUrl}",          encodedRequestUrl);
            _logger?.LogDebug("access_token = {EncodedAccessToken}",        encodedAccessToken);
            _logger?.LogDebug("proof-key.value = {PublicKeyCspBlob}",       publicKeyCspBlob);
            _logger?.LogDebug("proof-key.oldvalue = {OldPublicKeyCspBlob}", oldPublicKeyCspBlob);
            _logger?.LogDebug("X-WOPI-Timestamp = {Timestamp}",             timestamp);
            _logger?.LogDebug("X-WOPI-Proof = {GivenProof}",                givenProof);
            _logger?.LogDebug("X-WOPI-ProofOld = {OldGivenProof}",          oldGivenProof);

            // Is the proof verifiable using either our current key or the old one?  If not, maybe there is a new key that we 
            // do not know about, thus we might be able to verify using the old proof with our current key (ie our current key is old
            // but we are still working with a now outdated discovery document which we need to refresh).

            if (IsProven(expectedProof, givenProof, publicKeyCspBlob))    return (PROOF_IS_VALID, refetchProofKeys: false);    // discovery doc is the latest
            if (IsProven(expectedProof, oldGivenProof, publicKeyCspBlob)) return (PROOF_IS_VALID, refetchProofKeys: true);     // discovery doc needs to be refreshed

            // Next scenario is one where our discovery document is up to date, but the proof was generated using an old key and if 
            // that doesn't work then using the old key to sign the old proof but having the new key fail to validate the new proof
            // smacks of dodgy shenanigans so I guess we'll just let that one fail to be on the safe side

            if (IsProven(expectedProof, givenProof, oldPublicKeyCspBlob)) return (PROOF_IS_VALID, refetchProofKeys: false);

            // There is a scenario that is impossible for us to distinguish from a potential attack, and that is the one where 
            // the WOPI client has rotated the keys mutliple times since we last refreshed the discovery document or doesn't correctly implement
            // key rotation and thus is using a key we don't know about.
            // Safest thing for us to do is to refetch the document whenever something fails validation and mitigate the DDoS vector this opens up 
            // at the infrastructure level (we'll also try to limit the number of requests we make, but we're limited by size of server farm we are
            // hosted is with respect to how effective this can be.

            // Another possibility is the Collabora Docker Container has been updated and all the signing keys have been recycled.  No way for us to 
            // easily determine that so let's just go get the latest discovery document to make sure.  NB - this should be mitigated by injecting the 
            // same proof key into the deployed container so all instances in a farm use the same one.   Won't necessarily be doing this in a local 
            // dev environment though if running direct against a standard collabora:latest image.

            _logger?.LogWarning("WOPI proof(s) failed to validate.  On the assumption this is a valid request and our keys our out of sync, an attempt will be made to retrieve any 'new' one by flushing our cache.");

            //throw new ApplicationException($"FAILED TO VERIFY PROOF: location_header = '{locationHeader ?? "null"}' request_url = '{encodedRequestUrl}', access_token = '{encodedAccessToken}', timestamp = '{timestamp}', given-proof = '{givenProof}', old-given-proof = '{oldGivenProof}' proof-key = '{publicKeyCspBlob}'");

            return (PROOF_IS_INVALID, refetchProofKeys: true);
        }

        /// <summary>
        /// Tasked with verifying that the presented proof does indeed match that which we would have expected the 
        /// trusted WOPI client (from which we pulled the Discovery Document we represent) to have produced for the 
        /// specific request that has been made to us (the WOPI Host).  This is the approach taken to ensure the request
        /// validity is non-repudiable
        /// </summary>
        /// <param name="expectedProof">The proof we would have expected the trusted client to offer</param>
        /// <param name="signedProof">The proof presented to ourselves that needs to be verified</param>
        /// <param name="publicKeyCspBlob">The CSP Blob the trusted WOPI client is thought to have used to sign the proof source</param>
        /// <returns></returns>
        private static bool IsProven(byte[] expectedProof, string signedProof, string? publicKeyCspBlob)
        {
            Debug.Assert(expectedProof is not null && 0 < expectedProof.Length);
            Debug.Assert(!string.IsNullOrWhiteSpace(signedProof));

            if (string.IsNullOrWhiteSpace(publicKeyCspBlob)) throw new ArgumentNullException(nameof(publicKeyCspBlob));

            const bool HAS_NOT_BEEN_VERIFIED = false;

            const string SHA256 = "SHA256";

            var publicKey = Convert.FromBase64String(publicKeyCspBlob);

            var proof = Convert.FromBase64String(signedProof);

            try
            {
                using var rsaAlgorithm = new RSACryptoServiceProvider();

                rsaAlgorithm.ImportCspBlob(publicKey);

                return rsaAlgorithm.VerifyData(expectedProof, SHA256, proof);
            }
            catch (FormatException) { return HAS_NOT_BEEN_VERIFIED; }
            catch (CryptographicException) { return HAS_NOT_BEEN_VERIFIED; }
        }
    }
}

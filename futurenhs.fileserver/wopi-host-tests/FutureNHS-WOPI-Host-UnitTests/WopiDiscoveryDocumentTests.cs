using FutureNHS.WOPIHost;
using FutureNHS.WOPIHost.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace FutureNHS_WOPI_Host_UnitTests
{
    [TestClass]
    public sealed class WopiDiscoveryDocumentTests
    {
        internal const string WOPI_ROOT = "https://futurenhs.cds.co.uk/gateway/wopi/";
        internal const string WOPI_FILE_SRC = WOPI_ROOT + "host/files/documentnamegoeshere.docx";
        internal const string WOPI_DISCOVERY_DOCUMENT_URL = WOPI_ROOT + "client/hosting/discovery";
        internal const string WOPI_DISCOVERY_DOCUMENT_XML = 
            "<wopi-discovery>" + 
              "<net-zone name=\"external-http\">" + 
                "<app name=\"writer\">" + 
                  "<action default=\"true\" ext=\"docx\" name=\"edit\" urlsrc=\"" + WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?\"></action>" + 
                "</app>" + 
              "</net-zone>" +
              "<proof-key exponent=\"AQAB\" " +
                         "modulus=\"0TAzPoRjdY14NelqBGwJTnrFHuAuowoxUJvIayDZhi6DhyPUPYTT/zM9o5MJC4PVl7EduQSqTRunjZobFuDp7zMX9HmzkAwTZn07tpxEL7jYIy8a2+Rkl7KqEE0LQVCWQxS41vXOtxCgY81BJalnHEKXMQpO1Emjc2EBoOTJBPjf4iUILqMKE+RhKqs/ifXD/4n9IZzErAiIrQK93EsOhHqIabY/LDPpI2B4rNbnlC8Gf+HvFWkul/UsLdw6RcSjP3vGrHfYORmIafzWooAtVcTx2HAeydLgGlQq9DvahvoVB0rz3sYOmPW+L45a12t57ig1V/T+fuukYqBg2XTbxQ==\" " +
                         "oldexponent=\"AQAB\" " +
                         "oldmodulus=\"0TAzPoRjdY14NelqBGwJTnrFHuAuowoxUJvIayDZhi6DhyPUPYTT/zM9o5MJC4PVl7EduQSqTRunjZobFuDp7zMX9HmzkAwTZn07tpxEL7jYIy8a2+Rkl7KqEE0LQVCWQxS41vXOtxCgY81BJalnHEKXMQpO1Emjc2EBoOTJBPjf4iUILqMKE+RhKqs/ifXD/4n9IZzErAiIrQK93EsOhHqIabY/LDPpI2B4rNbnlC8Gf+HvFWkul/UsLdw6RcSjP3vGrHfYORmIafzWooAtVcTx2HAeydLgGlQq9DvahvoVB0rz3sYOmPW+L45a12t57ig1V/T+fuukYqBg2XTbxQ==\" " +
                         "oldvalue=\"BgIAAACkAABSU0ExAAgAAAEAAQDF23TZYKBipOt+/vRXNSjueWvXWo4vvvWYDsbe80oHFfqG2jv0KlQa4NLJHnDY8cRVLYCi1vxpiBk52Hesxns/o8RFOtwtLPWXLmkV7+F/Bi+U59aseGAj6TMsP7ZpiHqEDkvcvQKtiAisxJwh/Yn/w/WJP6sqYeQTCqMuCCXi3/gEyeSgAWFzo0nUTgoxl0IcZ6klQc1joBC3zvXWuBRDllBBC00QqrKXZOTbGi8j2LgvRJy2O31mEwyQs3n0FzPv6eAWG5qNpxtNqgS5HbGX1YMLCZOjPTP/04Q91COHgy6G2SBryJtQMQqjLuAexXpOCWwEauk1eI11Y4Q+MzDR\" " +
                         "value=\"BgIAAACkAABSU0ExAAgAAAEAAQDF23TZYKBipOt+/vRXNSjueWvXWo4vvvWYDsbe80oHFfqG2jv0KlQa4NLJHnDY8cRVLYCi1vxpiBk52Hesxns/o8RFOtwtLPWXLmkV7+F/Bi+U59aseGAj6TMsP7ZpiHqEDkvcvQKtiAisxJwh/Yn/w/WJP6sqYeQTCqMuCCXi3/gEyeSgAWFzo0nUTgoxl0IcZ6klQc1joBC3zvXWuBRDllBBC00QqrKXZOTbGi8j2LgvRJy2O31mEwyQs3n0FzPv6eAWG5qNpxtNqgS5HbGX1YMLCZOjPTP/04Q91COHgy6G2SBryJtQMQqjLuAexXpOCWwEauk1eI11Y4Q+MzDR\" " +
              "/>" +
            "</wopi-discovery>";

#if DEBUG

        [TestMethod]
        [DataRow(WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?", WOPI_ROOT + "host/files/filenamegoeshere.docx", WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?")]
        [DataRow(WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?<UNKNOWN_PLACEHOLDER=PLACEHOLDER_VALUE>", WOPI_ROOT + "host/files/filenamegoeshere.docx", WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?")]
        [DataRow(WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?<UNKNOWN_PLACEHOLDER=PLACEHOLDER_VALUE[&]><UNKNOWN_PLACEHOLDER=PLACEHOLDER_VALUE>", WOPI_ROOT + "host/files/filenamegoeshere.docx", WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?")]
        [DataRow(WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?<WOPI_SRC=PLACEHOLDER_VALUE>", WOPI_ROOT + "host/files/filenamegoeshere.docx", WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?WOPI_SRC=" + WOPI_ROOT + "host/files/filenamegoeshere.docx")]
        [DataRow(WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?<WOPI_SRC=PLACEHOLDER_VALUE[&]>", WOPI_ROOT + "host/files/filenamegoeshere.docx", WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?WOPI_SRC=" + WOPI_ROOT + "host/files/filenamegoeshere.docx")]
        [DataRow(WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?<WOPI_SRC=PLACEHOLDER_VALUE[&]><WOPI_SRC=PLACEHOLDER_VALUE>", WOPI_ROOT + "host/files/filenamegoeshere.docx", WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?WOPI_SRC=" + WOPI_ROOT + "host/files/filenamegoeshere.docx&WOPI_SRC=" + WOPI_ROOT + "host/files/filenamegoeshere.docx")]
        [DataRow(WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?<WOPI_SRC=PLACEHOLDER_VALUE[&]><UNKNOWN_PLACEHOLDER=PLACEHOLDER_VALUE>", WOPI_ROOT + "host/files/filenamegoeshere.docx", WOPI_ROOT + "client/loleaflet/4aa2794/loleaflet.html?WOPI_SRC=" + WOPI_ROOT + "host/files/filenamegoeshere.docx")]
        public void TransformActionUrlSrcAttribute_CorrectlyReplacesAndRemovesPlaceholders(string urlSrc, string wopiSrc, string expectedUrlSrc)
        {
            var transformedUrlSrc = WopiDiscoveryDocument.TransformActionUrlSrcAttribute(urlSrc, new Uri(wopiSrc, UriKind.Absolute));

            Assert.AreEqual(expectedUrlSrc, transformedUrlSrc);
        }

#endif


        [TestMethod]
        [ExpectedException(typeof(WopiDiscoveryDocumentEmptyException))]
        public void GetEndpointForFileExtension_ThrowsIfDocumentIsEmpty()
        {
            IWopiDiscoveryDocument wopiDiscoveryDocument = WopiDiscoveryDocument.Empty;

            _ = wopiDiscoveryDocument.GetEndpointForFileExtension("docx", "edit", new Uri(WOPI_FILE_SRC, UriKind.Absolute));
        }

        [TestMethod]
        public void GetEndpointForFileExtension_ReturnsCorrectEndpointForBothKnownAndUnknownFileExtensions()
        {
            var logger = new Moq.Mock<ILogger>().Object;

            var sourceEndpoint = new Uri(WOPI_ROOT + "client/hosting/discovery", UriKind.Absolute);

            var xml = XDocument.Parse(WOPI_DISCOVERY_DOCUMENT_XML, LoadOptions.None);

            IWopiDiscoveryDocument wopiDiscoveryDocument = new WopiDiscoveryDocument(sourceEndpoint, xml, logger);

            var endpoint = wopiDiscoveryDocument.GetEndpointForFileExtension("docx", "edit", new Uri(WOPI_FILE_SRC, UriKind.Absolute));

            Assert.IsNotNull(endpoint, "Expected the endpoint to be returned when it is supported by the wopi client");
            Assert.IsTrue(endpoint.IsAbsoluteUri, "The endpoint should be an absolute uri for supported file extensions");

            endpoint = wopiDiscoveryDocument.GetEndpointForFileExtension("fakefileextension", "edit", new Uri(WOPI_FILE_SRC, UriKind.Absolute));

            Assert.IsNull(endpoint, "Expected a null return value when the file extension is not supported by the wopi client");
        } 
    }
}

namespace FutureNHS.Api.Authorization
{
	using AspNetCore.Authentication.ApiKey;
    using System.Security.Claims;

    public class ApiKey : IApiKey
	{
		public ApiKey(string key, string owner, List<Claim> claims = null)
		{
			Key = key;
			OwnerName = owner;
			Claims = claims ?? new List<Claim>();
		}

		public string Key { get; }
		public string OwnerName { get; }
		public IReadOnlyCollection<Claim> Claims { get; }
	}
}

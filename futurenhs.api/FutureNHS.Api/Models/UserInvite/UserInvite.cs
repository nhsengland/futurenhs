namespace FutureNHS.Api.Models.UserInvite
{
    public record UserInvite
    {
      public string? EmailAddress { get; init; }
      public string?  GroupSlug { get; init; }
    }
}

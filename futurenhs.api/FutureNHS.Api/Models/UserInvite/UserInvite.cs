namespace FutureNHS.Api.Models.UserInvite
{
    public record UserInvite
    {
      public string? EmailAddress { get; init; }
      public Guid?  GroupId { get; init; }
    }
}

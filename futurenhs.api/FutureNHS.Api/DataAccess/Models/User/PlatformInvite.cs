using MimeDetective.Storage.Xml.v2;

namespace FutureNHS.Api.DataAccess.Models.User;

public record PlatformInvite : BaseData
{
    public Guid Id { get; init; }
    
    public string EmailAddress { get; init; }
    public Guid? GroupId { get; init; }
    public Guid CreatedBy { get; init; }
    public DateTime CreatedAtUTC { get; init; }
    public DateTime ExpiresAtUTC { get; init; }
}
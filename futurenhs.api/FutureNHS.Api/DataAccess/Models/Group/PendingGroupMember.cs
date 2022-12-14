using FutureNHS.Api.DataAccess.Models.Registration;

namespace FutureNHS.Api.DataAccess.Models.Group
{
    public sealed record PendingGroupMember
    {
            
        public Guid Id { get; init; }
        
        public Guid? UserId { get; init; }
        
        public string Email { get; init; }
        
        public string CreatedAtUTC { get; init; }
        
        public string InviteType { get; init; }
        
        public byte[] RowVersion { get; init; }


    }
}
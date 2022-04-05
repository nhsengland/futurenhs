namespace FutureNHS.Api.DataAccess.DTOs
{
    public class GroupSiteDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid ContentRootId { get; set; }
        public Guid? CreatedBy { get; init; }
        public DateTime CreatedAtUTC { get; init; }
        public Guid? ModifiedBy { get; init; }
        public DateTime? ModifiedAtUTC { get; init; }
    }
}

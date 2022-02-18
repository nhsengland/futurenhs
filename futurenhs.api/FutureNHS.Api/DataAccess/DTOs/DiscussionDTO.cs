namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed class DiscussionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedAtUTC { get; set; }
        public Guid CreatedBy { get; set; }
        public bool IsSticky { get; set; }
        public bool IsLocked { get; set; }
        public Guid GroupId { get; set; }
        public Guid? PollId { get; set; }
        public Guid? CategoryId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}

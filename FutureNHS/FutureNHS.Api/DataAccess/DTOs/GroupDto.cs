namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed class GroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Subtitle { get; set; }
        public string Introduction { get; set; }
        public bool IsLocked { get; set; }
        public bool? ModerateTopics { get; set; }
        public bool? ModeratePosts { get; set; }
        public int SortOrder { get; set; }
        public DateTime DateCreated { get; set; }
        public string Slug { get; set; }
        public string PageTitle { get; set; }
        public string Path { get; set; }
        public string MetaDescription { get; set; }
        public string Colour { get; set; }
        public string Image { get; set; }
        public Guid  GroupOwner { get; set; }
        public int Level { get; set; }
        public string AboutUs { get; set; }
        public Guid? ImageId { get; set; }
        public bool PublicGroup { get; set; }
        public bool HiddenGroup { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] RowVersion { get; set; }
    }
}

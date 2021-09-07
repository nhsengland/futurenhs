namespace MvcForum.Core.Data.Mapping
{
    using Models.Entities;
    using System.Data.Entity.ModelConfiguration;

    public class GroupInviteMapping : EntityTypeConfiguration<GroupInvite>
    {
        public GroupInviteMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.EmailAddress).IsRequired().HasMaxLength(254);
            Property(x => x.IsDeleted).IsRequired();
            Property(x => x.GroupId).IsRequired();
        }
    }
}
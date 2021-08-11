namespace MvcForum.Core.Data.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    using Models.Entities;

    public class GroupPermissionForRoleMapping : EntityTypeConfiguration<GroupPermissionForRole>
    {
        public GroupPermissionForRoleMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.IsTicked).IsRequired();
            HasRequired(x => x.Group).WithMany(x => x.GroupPermissionForRoles).Map(x => x.MapKey("Group_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.Permission).WithMany(x => x.GroupPermissionForRoles).Map(x => x.MapKey("Permission_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.MembershipRole).WithMany(x => x.GroupPermissionForRoles).Map(x => x.MapKey("MembershipRole_Id")).WillCascadeOnDelete(false);
        }
    }
}

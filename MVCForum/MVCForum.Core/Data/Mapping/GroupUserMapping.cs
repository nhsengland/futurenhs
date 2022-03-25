namespace MvcForum.Core.Data.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    using Models.Entities;

    public class GroupUserMapping : EntityTypeConfiguration<GroupUser>
    {
        public GroupUserMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            
            HasRequired(x => x.Group)
            .WithMany()
            .Map(x => x.MapKey("Group_Id"));

            HasRequired(x => x.User)
                .WithMany()
                .Map(x => x.MapKey("MembershipUser_Id"));

            Property(x => x.Approved).IsRequired();
            Property(x => x.Rejected).IsRequired();
            Property(x => x.Locked).IsRequired();
            Property(x => x.Banned).IsRequired();
        

            HasOptional(x => x.Role)
                .WithMany()
                .Map(x => x.MapKey("MembershipRole_Id"))
                .WillCascadeOnDelete(false);

            HasOptional(x => x.ApprovingUser)
                .WithMany()
                .Map(x => x.MapKey("ApprovingMembershipUser_Id"));

            Property(x => x.RequestToJoinDateUTC).IsRequired();
            Property(x => x.ApprovedToJoinDateUTC).IsOptional();
            Property(x => x.RequestToJoinReason).IsOptional().HasMaxLength(200);
            Property(x => x.LockReason).IsOptional().HasMaxLength(200);
            Property(x => x.BanReason).IsOptional().HasMaxLength(200);
        }
    }
}

namespace MvcForum.Core.Data.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    using Models.Entities;

    public class GroupNotificationMapping : EntityTypeConfiguration<GroupNotification>
    {
        public GroupNotificationMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();

            HasRequired(x => x.Group)
                .WithMany(x => x.GroupNotifications)
                .Map(x => x.MapKey("Group_Id"))
                .WillCascadeOnDelete(false);

        }
    }
}

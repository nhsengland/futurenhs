namespace MvcForum.Core.Data.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    using Models.Activity;

    public class ActivityMapping : EntityTypeConfiguration<Activity>
    {
        public ActivityMapping()
        {
            HasKey(x => x.Id);

            Property(x => x.Id).IsRequired();
            Property(x => x.Timestamp).IsRequired();
            Property(x => x.Data).IsRequired();
            Property(x => x.Type).IsRequired().HasMaxLength(50);

            // TODO - Change Table Names
            //ToTable("Activity"); 
        }
    }
}
namespace MvcForum.Core.Data.Mapping
{
    using Models.Entities;
    using System.Data.Entity.ModelConfiguration;

    public class SystemPageMapping : EntityTypeConfiguration<SystemPage>
    {
        public SystemPageMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Slug).IsRequired();
            Property(x => x.Title).IsRequired();
            Property(x => x.Content).IsRequired();
            Property(x => x.IsDeleted).IsRequired();
    
        }
    }
}
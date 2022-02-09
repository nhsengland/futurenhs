namespace MvcForum.Core.Data.Mapping
{
    using Models.Entities;
    using System.Data.Entity.ModelConfiguration;

    public class ImageMapping : EntityTypeConfiguration<Image>
    {
        public ImageMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.FileName).IsRequired().HasMaxLength(45);
            Property(x => x.FileSizeBytes).IsRequired();
            Property(x => x.Height).IsRequired();
            Property(x => x.Width).IsRequired();
            Property(x => x.MediaType).IsRequired().HasMaxLength(20);
            Property(x => x.IsDeleted).IsRequired();
        }
    }
}
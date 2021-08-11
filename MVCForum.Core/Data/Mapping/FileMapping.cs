namespace MvcForum.Core.Data.Mapping
{
    using Models.Entities;
    using System.Data.Entity.ModelConfiguration;

    public class FileMapping : EntityTypeConfiguration<File>
    {
        public FileMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Title).IsRequired();
            Property(x => x.ParentFolder).IsRequired();
            Property(x => x.Description).IsOptional();
            Property(x => x.FileName).IsRequired();
            Property(x => x.FileSize).IsRequired();
            Property(x => x.FileExtension).IsRequired();
            Property(x => x.FileUrl).IsRequired();
            Property(x => x.CreatedBy).IsRequired();
            Property(x => x.ModifiedBy).IsRequired();
            Property(x => x.CreatedDate).IsRequired();
            Property(x => x.ModifiedDate).IsRequired();
        }
    }
}
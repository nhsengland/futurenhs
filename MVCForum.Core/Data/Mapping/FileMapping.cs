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
            Property(x => x.FileName).IsOptional();
            Property(x => x.FileSize).IsOptional();
            Property(x => x.FileExtension).IsOptional();
            Property(x => x.FileUrl).IsOptional();
            Property(x => x.BlobHash).IsOptional();            
            Property(x => x.CreatedBy).IsRequired();
            Property(x => x.ModifiedBy).IsOptional();
            Property(x => x.CreatedDate).IsRequired();
            Property(x => x.ModifiedDate).IsOptional();
            Property(x => x.UploadStatus).IsOptional();
        }
    }
}
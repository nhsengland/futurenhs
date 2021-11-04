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
            Property(x => x.Title).IsRequired().HasMaxLength(30);
            Property(x => x.ParentFolder).IsRequired();
            Property(x => x.Description).IsOptional().HasMaxLength(250);
            Property(x => x.FileName).IsOptional().HasMaxLength(30);
            Property(x => x.FileSizeBytes).IsOptional();
            Property(x => x.FileExtension).IsOptional().HasMaxLength(10);
            Property(x => x.BlobName).IsOptional().HasMaxLength(100);
            Property(x => x.BlobHash).IsOptional().HasMaxLength(16).IsFixedLength();            
            Property(x => x.CreatedBy).IsRequired();
            Property(x => x.ModifiedBy).IsOptional();
            Property(x => x.CreatedAtUtc).IsRequired().HasColumnType("datetime2");
            Property(x => x.ModifiedAtUtc).IsOptional().HasColumnType("datetime2");
            Property(x => x.FileStatus).IsOptional();
        }
    }
}
namespace MvcForum.Core.Data.Mapping
{
    using Models.Entities;
    using System.Data.Entity.ModelConfiguration;

    public class FolderMapping : EntityTypeConfiguration<Folder>
    {
        public FolderMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Name).IsRequired().HasMaxLength(1000);
            Property(x => x.Description).IsOptional().HasMaxLength(4000);
            Property(x => x.FileCount).IsRequired();
            Property(x => x.CreatedAtUtc).IsRequired();
            Property(x => x.ParentFolder).IsOptional();
            Property(x => x.AddedBy).IsRequired();
            Property(x => x.ParentGroup).IsRequired();
            Property(x => x.IsDeleted).IsRequired();
            Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
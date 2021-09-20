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
            Property(x => x.Name).IsRequired().HasMaxLength(450);
            Property(x => x.Description).IsOptional();
            Property(x => x.FileCount).IsRequired();
            Property(x => x.DateAdded).IsRequired();
            Property(x => x.ParentFolder).IsOptional();
            Property(x => x.AddedBy).IsRequired();
            Property(x => x.ParentGroup).IsRequired();
            Property(x => x.IsDeleted).IsRequired();
            Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
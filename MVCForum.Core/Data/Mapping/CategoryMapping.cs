namespace MvcForum.Core.Data.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    using Models.Entities;

    public class CategoryMapping : EntityTypeConfiguration<Category>
    {
        public CategoryMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Name).IsRequired();
            Property(x => x.Description).IsRequired();
            Property(x => x.SortOrder).IsRequired();
            Property(x => x.DateCreated).IsRequired();
            HasRequired(x => x.Group).WithMany().Map(x => x.MapKey("Group_Id")).WillCascadeOnDelete(false);

            HasMany(x => x.Topics)
             .WithOptional(x => x.Category)
             .Map(x => x.MapKey("Category_Id"))
             .WillCascadeOnDelete(false);
        }
    }
}

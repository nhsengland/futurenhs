namespace MvcForum.Core.Data.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.ModelConfiguration;
    using Models.Entities;

    public class GroupMapping : EntityTypeConfiguration<Group>
    {
        public GroupMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Name).IsRequired().HasMaxLength(450);
            Property(x => x.Description).IsOptional();
            Property(x => x.Subtitle).IsOptional().HasMaxLength(254);
            Property(X => X.Introduction).IsRequired().HasMaxLength(4000);
            Property(x => x.DateCreated).IsRequired();
            Property(x => x.Slug).IsRequired().HasMaxLength(450)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Group_Slug", 1) {IsUnique = true}));
            Property(x => x.SortOrder).IsRequired();
            Property(x => x.IsLocked).IsRequired();
            Property(x => x.ModerateTopics).IsRequired();
            Property(x => x.ModeratePosts).IsRequired();
            Property(x => x.PageTitle).IsOptional().HasMaxLength(80);
            Property(x => x.MetaDescription).IsOptional().HasMaxLength(200);
            Property(x => x.Path).IsOptional().HasMaxLength(2500);
            Property(x => x.Colour).IsOptional().HasMaxLength(50);
            Property(x => x.Image).IsOptional().HasMaxLength(200);
            

            HasOptional(x => x.ParentGroup)
                .WithMany()
                .Map(x => x.MapKey("Group_Id"));

            HasOptional(x => x.Section)
                .WithMany(x => x.Groups)
                .Map(x => x.MapKey("Section_Id"))
                .WillCascadeOnDelete(false);

            HasOptional(x => x.GroupOwner)
                .WithMany()
                .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);

            HasMany(x => x.GroupNotifications)
                .WithRequired(x => x.Group)
                .Map(x => x.MapKey("Group_Id"))
                .WillCascadeOnDelete(false);

            HasMany(x => x.Categories)
                .WithRequired(x => x.Group)
                .Map(x => x.MapKey("Group_Id"))
                .WillCascadeOnDelete(false);


            HasMany(x => x.GroupUsers)
                .WithRequired(x => x.Group)
                .Map(x => x.MapKey("Group_Id"))
                .WillCascadeOnDelete(false);


            Property(x => x.IsDeleted).IsRequired();
            Property(x => x.RowVersion).IsRowVersion();

            // Ignores
            Ignore(x => x.NiceUrl);
            Ignore(x => x.Level);
        }
    }
}
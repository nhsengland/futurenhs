namespace MvcForum.Core.Data.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.ModelConfiguration;
    using Models.Entities;

    public class MembershipUserMapping : EntityTypeConfiguration<MembershipUser>
    {
        public MembershipUserMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.UserName).IsRequired().HasMaxLength(150)
                                    .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                                    new IndexAnnotation(new IndexAttribute("IX_MembershipUser_UserName", 1) { IsUnique = true }));
            Property(x => x.Password).IsRequired().HasMaxLength(128);
            Property(x => x.PasswordSalt).IsOptional().HasMaxLength(128);
            Property(x => x.Email).IsOptional().HasMaxLength(256);
            Property(x => x.PasswordQuestion).IsOptional().HasMaxLength(256);
            Property(x => x.PasswordAnswer).IsOptional().HasMaxLength(256);
            Property(x => x.IsApproved).IsRequired();
            Property(x => x.IsLockedOut).IsRequired();
            Property(x => x.IsBanned).IsRequired();
            Property(x => x.CreatedAtUTC).IsRequired();
            Property(x => x.LastLoginDateUTC).IsOptional();
            Property(x => x.LastPasswordChangedDateUTC).IsOptional();
            Property(x => x.LastLockoutDateUTC).IsOptional();
            Property(x => x.FailedPasswordAttemptCount).IsRequired();
            Property(x => x.FailedPasswordAnswerAttempt).IsRequired();
            Property(x => x.PasswordResetToken).HasMaxLength(150).IsOptional();
            Property(x => x.PasswordResetTokenCreatedAtUTC).IsOptional();
            Property(x => x.Slug).IsRequired().HasMaxLength(150)
                                    .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                                    new IndexAnnotation(new IndexAttribute("IX_MembershipUser_Slug", 1) { IsUnique = true }));
 
            Property(x => x.IsExternalAccount).IsOptional();
            Property(x => x.LoginIdExpiresUTC).IsOptional();
            Property(x => x.LastActivityDateUTC).IsOptional();
            Property(x => x.HasAgreedToTermsAndConditions).IsOptional();
            Property(x => x.Pronouns).IsOptional().HasMaxLength(255);
            Property(x => x.ImageId).IsOptional();

            Ignore(x => x.TotalPoints);
            Ignore(x => x.NiceUrl);

            HasMany(x => x.Topics).WithRequired(x => x.User)
                .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);

            HasMany(x => x.UploadedFiles).WithRequired(x => x.MembershipUser)
                .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);

            // Has Many, as a user has many posts
            HasMany(x => x.Posts).WithRequired(x => x.User)
               .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);

            HasMany(x => x.Votes).WithRequired(x => x.User)
               .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);

            HasMany(x => x.VotesGiven).WithOptional(x => x.VotedByMembershipUser)
                .Map(x => x.MapKey("VotedByMembershipUser_Id"));

            HasMany(x => x.TopicNotifications).WithRequired(x => x.User)
               .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);

            HasMany(x => x.Polls).WithRequired(x => x.User)
               .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);

            HasMany(x => x.PollVotes).WithRequired(x => x.User)
               .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);

            HasMany(x => x.GroupNotifications).WithRequired(x => x.User)
               .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);

            HasMany(x => x.TagNotifications).WithRequired(x => x.User)
            .Map(x => x.MapKey("MembershipUser_Id"))
            .WillCascadeOnDelete(false);

            HasMany(x => x.Points).WithRequired(x => x.User)
               .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);


            // Many-to-many join table - a user may belong to many roles
            HasMany(t => t.Roles)
            .WithMany(t => t.Users)
            .Map(m =>
            {
                m.ToTable("MembershipUsersInRoles");
                m.MapLeftKey("UserIdentifier");
                m.MapRightKey("RoleIdentifier");
            });
           

            HasMany(x => x.PostEdits)
                .WithRequired(x => x.EditedBy)
                .Map(x => x.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);

        }
    }
}

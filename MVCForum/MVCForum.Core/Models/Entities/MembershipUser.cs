using System.ComponentModel.DataAnnotations.Schema;

namespace MvcForum.Core.Models.Entities
{
    using MvcForum.Core.ExtensionMethods;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enums;
    using General;
    using Interfaces;
    using Utilities;

    /// <summary>
    ///     A membership user
    /// </summary>
    public partial class MembershipUser : IBaseEntity
    {
        public MembershipUser()
        {
            Id = GuidComb.GenerateComb();

            // Default size of 100 for now, override with something else if required
            MemberImageSize = 100;
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Initials { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreatedAtUTC { get; set; }
        public DateTime? LastLoginDateUTC { get; set; }
        public DateTime? LastPasswordChangedDateUTC { get; set; }
        public DateTime? LastLockoutDateUTC { get; set; }
        public DateTime? LastActivityDateUTC { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public int FailedPasswordAnswerAttempt { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenCreatedAtUTC { get; set; }
        public string Slug { get; set; }
        public bool? IsExternalAccount { get; set; }
        public DateTime? LoginIdExpiresUTC { get; set; }

        public bool? HasAgreedToTermsAndConditions { get; set; }

        public bool IsTrustedUser { get; set; }

        public string Pronouns { get; set; }

        public Guid? ImageId { get; set; }

        public virtual IList<MembershipRole> Roles { get; set; }
        public virtual IList<Post> Posts { get; set; }
        public virtual IList<Topic> Topics { get; set; }
        public virtual IList<Vote> Votes { get; set; }
        public virtual IList<Vote> VotesGiven { get; set; }
        public virtual IList<GroupNotification> GroupNotifications { get; set; }
        public virtual IList<TopicNotification> TopicNotifications { get; set; }
        public virtual IList<TagNotification> TagNotifications { get; set; }
        public virtual IList<MembershipUserPoints> Points { get; set; }
        public virtual IList<Poll> Polls { get; set; }
        public virtual IList<PollVote> PollVotes { get; set; }
        public virtual IList<Favourite> Favourites { get; set; }
        public virtual IList<UploadedFile> UploadedFiles { get; set; }
        public virtual IList<Block> BlockedUsers { get; set; }
        public virtual IList<Block> BlockedByOtherUsers { get; set; }
        public virtual IList<PostEdit> PostEdits { get; set; }

        public int TotalPoints
        {
            get { return Points?.Select(x => x.Points).Sum() ?? 0; }
        }

        public string GetFullName()
        {
            var fullName = $"{this.FirstName} {this.Surname}".CapitaliseEachWord();

            return string.IsNullOrWhiteSpace(fullName) ? this.UserName : fullName;
        }

        public string NiceUrl => UrlTypes.GenerateUrl(UrlType.Member, Slug);

        [NotMapped]
        public string CustomClassName { get; set; }
        [NotMapped]
        public int MemberImageSize { get; set; }
    }
}
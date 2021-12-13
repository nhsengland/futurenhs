using System.ComponentModel.DataAnnotations;

namespace MvcForum.Core.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Enums;
    using Interfaces;
    using Utilities;

    public partial class Group : ExtendedDataEntity, IBaseEntity
    {
        public Group()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Subtitle { get; set; }
        public string Introduction { get; set; }
        public bool IsLocked { get; set; }
        public bool? ModerateTopics { get; set; }
        public bool? ModeratePosts { get; set; }
        public int SortOrder { get; set; }
        public DateTime DateCreated { get; set; }
        public string Slug { get; set; }
        public string PageTitle { get; set; }
        public string Path { get; set; }
        public string MetaDescription { get; set; }
        public string Colour { get; set; }
        public string Image { get; set; }
        public virtual Group ParentGroup { get; set; }
        public virtual Section Section { get; set; }
        public virtual IList<GroupUser> GroupUsers { get; set; }
        public virtual IList<Topic> Topics { get; set; }
        public virtual IList<Category> Categories { get; set; }
        public virtual IList<GroupNotification> GroupNotifications { get; set; }
        public virtual IList<GroupPermissionForRole> GroupPermissionForRoles { get; set; }
        public virtual MembershipUser GroupOwner { get; set; }
        public int Level { get; set; }
        public string AboutUs { get; set; }


        public bool PublicGroup { get; set; }
        public bool HiddenGroup { get; set; }
        public bool IsDeleted { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public string NiceUrl => UrlTypes.GenerateUrl(UrlType.Group, Slug);
    }

}

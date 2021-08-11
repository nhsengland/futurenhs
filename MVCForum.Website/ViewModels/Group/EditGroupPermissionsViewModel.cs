namespace MvcForum.Web.ViewModels.Group
{
    using System.Collections.Generic;
    using Core.Models.Entities;

    public class EditGroupPermissionsViewModel
    {
        public Group Group { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<MembershipRole> Roles { get; set; }
    }
}
namespace MvcForum.Core.ExtensionMethods
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Models.Entities;

    public static class MembershipUserExtensions
    {
        /// <summary>
        /// Is this user an Admin
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsAdmin(this MembershipUser user)
        {
            return user.Roles.Any(x => x.RoleName.Contains(Constants.Constants.AdminRoleName));
        }

        public static IEnumerable<SelectListItem> ToSelectList(this IList<MembershipUser> users, bool addEmptyFirstItem = true)
        {
            var userList = users.Select(x => new SelectListItem {
                Value = x.Id.ToString(),
                Text = x.UserName
            }).ToList();

            if (addEmptyFirstItem) {
                userList.Insert(0, new SelectListItem { Value = "", Text = "" });
            }

            return userList;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcForum.Core.Models.Entities;
using MvcForum.Core.Models.Enums;
using MvcForum.Web.ViewModels.Topic;

namespace MvcForum.Web.ViewModels.Group
{
    public class GroupTopicsViewModel
    {
        public List<TopicViewModel> Topics { get; set; }
        public MembershipRole LoggedInUserRole { get; set; }
        public  Guid GroupId { get; set; }
        public int TopicCount { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
        public GroupUserStatus GroupUserStatus { get; set; }
        public bool PublicGroup { get; set; }
    }
}
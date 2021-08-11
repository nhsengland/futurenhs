using System;

namespace MvcForum.Web.ViewModels.Topic
{
    using Core.Models.Entities;

    public class CreateTopicButtonViewModel
    {
        public Guid GroupId { get; set; }
        public MembershipUser LoggedOnUser { get; set; }
        public bool UserCanPostTopics { get; set; }
    }
}
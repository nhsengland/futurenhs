namespace MvcForum.Web.ViewModels.Post
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Constants;

    public class CreateAjaxPostViewModel
    {
        [UIHint(Constants.EditorType)]
        [AllowHtml]
        [StringLength(6000)]
        public string PostContent { get; set; }

        public Guid Topic { get; set; }
        public bool DisablePosting { get; set; }
        public Guid? Thread { get; set; }
        public Guid? InReplyTo { get; set; }
        public string ReplyToUsername { get; set; }
        public string ReplyToUsernameUrl { get; set; }
        public string CurrentUser { get; set; }
        
        public string CurrentUserUrl { get; set; }

        public string ReplyUserUrl { get; set; }

        /// <summary>
        /// Gets or sets an error on the view model (server side, no JS).
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Whether there is an error to display.
        /// </summary>
        public bool HasError 
        {
            get { return !string.IsNullOrEmpty(Error); }
        }
    }
}
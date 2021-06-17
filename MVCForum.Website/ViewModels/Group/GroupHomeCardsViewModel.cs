namespace MvcForum.Web.ViewModels.Group
{
    using MvcForum.Web.ViewModels.Shared;

    /// <summary>
    /// Defines the view model for the group cards navigaion.
    /// </summary>
    public class GroupHomeCardsViewModel
    {
        /// <summary>
        /// Gets or sets the Forum Card tab.
        /// </summary>
        public Tab ForumCard { get; set; }

        /// <summary>
        /// Gets or sets the Member Card tab.
        /// </summary>
        public Tab MembersCard { get; set; }
    }
}
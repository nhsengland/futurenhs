using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcForum.Web.ViewModels.Shared
{
    /// <summary>
    /// defines the View Model for the LinkGroup collection of links <see cref="NavItemBase"/>.
    /// </summary>
    public class LinkGroup : NavItemBase
    {

        /// <summary>
        /// Gets or sets the list of child items.
        /// </summary>
        public List<Link> ChildItems { get; set; }

        public string Icon { get; set; }
    }
}
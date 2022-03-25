using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcForum.Web.ViewModels.Shared
{

    /// <summary>
    /// Defines the base class for a navigation item e.g. a link or tab.
    /// </summary>
    public abstract class NavItemBase
    {
        private bool ActiveTab;
        /// <summary>
        /// Gets or sets the NavItem name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the url of the request.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the border theme.
        /// </summary>
        public string BorderTheme { get; set; }

        /// <summary>
        /// Gets or sets the icon theme
        /// </summary>
        public string IconTheme { get; set; }

        /// <summary>
        /// Gets or sets the order of the Nav item.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets boolean flag for active.
        /// </summary>
        public bool Active {
            get
            {
                if (!ActiveTab)
                {
                    return !string.IsNullOrEmpty(Url) && 
                        (
                            HttpContext.Current.Request.Url.LocalPath.Equals(Url) 
                            || 
                            (Url == "/" && HttpContext.Current.Request.Url.LocalPath.ToLower().StartsWith("/home/index"))
                        );
                }

                return ActiveTab;
            }

            set => ActiveTab = value;
        }
    }
}
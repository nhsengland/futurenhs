namespace MvcForum.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core;
    using Core.Constants;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteTable.Routes.LowercaseUrls = true;
            RouteTable.Routes.AppendTrailingSlash = false;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            // API Attribute Routes
            //routes.MapMvcAttributeRoutes();


            

            routes.MapRoute(
                "TermsAndConditionsUrl", // Route name
                "terms-and-conditions", // URL with parameters
                new { controller = "SystemPages", action = "Show", slug = "terms-and-conditions" } // Parameter defaults
            );

            routes.MapRoute(
                "PrivacyPolicyUrl", // Route name
                "privacy-policy", // URL with parameters
                new { controller = "SystemPages", action = "Show", slug = "privacy-policy" } // Parameter defaults
            );

            routes.MapRoute(
                "CookiesUrl", // Route name
                "cookies", // URL with parameters
                new { controller = "SystemPages", action = "Show", slug = "cookies" } // Parameter defaults
            );

            routes.MapRoute(
                "ContactUsUrl", // Route name
                "contact-us", // URL with parameters
                new { controller = "SystemPages", action = "Show", slug = "contact-us" } // Parameter defaults
            );
            routes.MapRoute(
                "AccessibilityStatementUrl", // Route name
                "accessibility-statement", // URL with parameters
                new { controller = "SystemPages", action = "Show", slug = "accessibility-statement" } // Parameter defaults
            );

            routes.MapRoute(
                "SystemPageUrls", // Route name
                "pages/{slug}", // URL with parameters
                new { controller = "SystemPages", action = "Show", slug = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "GroupUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/{slug}/{tab}"), // URL with parameters
                new { controller = "Group", action = "Show", slug = UrlParameter.Optional, tab = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "GroupFileUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/{slug}/{tab}/file/{id}"), // URL with parameters
                new { controller = "GroupFile", action = "Show", slug = UrlParameter.Optional, tab = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "GroupFileUploadUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/{slug}/{tab}/Upload/{folderId}"), // URL with parameters
                new { controller = "GroupFile", action = "Create", slug = UrlParameter.Optional, tab = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "GroupFileNewFolderUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/{slug}/{tab}/CreateFolder"), // URL with parameters
                new { controller = "Folder", action = "CreateFolder", slug = UrlParameter.Optional, tab = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "GroupFileEditFolderUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/{slug}/{tab}/UpdateFolder/{folderId}"), // URL with parameters
                new { controller = "Folder", action = "UpdateFolder", slug = UrlParameter.Optional, tab = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "GroupFileDeleteFolderUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/{slug}/{tab}/DeleteFolder/{folderId}"), // URL with parameters
                new { controller = "Folder", action = "DeleteFolder", slug = UrlParameter.Optional, tab = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "GroupInviteUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/{slug}/Invite/{groupId}"), // URL with parameters
                new { controller = "GroupInvite", action = "InviteMember", tab = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "GroupAddMemberUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/{slug}/{tab}/AddMember"), // URL with parameters
                new { controller = "GroupInvite", action = "AddMember", tab = "members" } // Parameter defaults
            );

            routes.MapRoute(
                "TopicUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/{group}/{tab}/{slug}"), // URL with parameters
                new { controller = "Topic", action = "Show", slug = UrlParameter.Optional } // Parameter defaults
            );            

            routes.MapRoute(
                "GroupMembersUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/ManageUsers/{slug}"), // URL with parameters
                new { controller = "Group", action = "ManageUsers", slug = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "GroupRssUrls", // Route name
                string.Concat(ForumConfiguration.Instance.GroupUrlIdentifier, "/rss/{slug}"), // URL with parameters
                new
                {
                    controller = "Group",
                    action = "GroupRss",
                    slug = UrlParameter.Optional
                } // Parameter defaults
            );

            //routes.MapRoute(
            //    "topicUrls", // Route name
            //    string.Concat(ForumConfiguration.Instance.TopicUrlIdentifier, "/{slug}"), // URL with parameters
            //    new {controller = "Topic", action = "Show", slug = UrlParameter.Optional} // Parameter defaults
            //);

            routes.MapRoute(
                "memberUrls", // Route name
                string.Concat(ForumConfiguration.Instance.MemberUrlIdentifier, "/{slug}"), // URL with parameters
                new { controller = "Members", action = "GetByName", slug = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "tagUrls", // Route name
                string.Concat(ForumConfiguration.Instance.TagsUrlIdentifier, "/{tag}"), // URL with parameters
                new { controller = "Topic", action = "TopicsByTag", tag = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "topicXmlSitemap", // Route name
                "topicxmlsitemap", // URL with parameters
                new { controller = "Home", action = "GoogleSitemap" } // Parameter defaults
            );

            routes.MapRoute(
                "GroupXmlSitemap", // Route name
                "Groupxmlsitemap", // URL with parameters
                new { controller = "Home", action = "GoogleGroupSitemap" } // Parameter defaults
            );

            routes.MapRoute(
                "memberXmlSitemap", // Route name
                "memberxmlsitemap", // URL with parameters
                new { controller = "Home", action = "GoogleMemberSitemap" } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{tab}", // URL with parameters
                new { controller = "Home", action = "Index", tab = UrlParameter.Optional } // Parameter defaults
            );
            //.RouteHandler = new SlugRouteHandler()
        }
    }
}

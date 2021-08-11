namespace MvcForum.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using ViewModels.Email;

    public partial class EmailController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly IGroupService _groupService;
        private readonly ITopicService _topicService;
        private readonly ITopicTagService _topicTagService;

        public EmailController(ILoggingService loggingService, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            INotificationService notificationService,
            IGroupService GroupService,
            ITopicService topicService, ITopicTagService topicTagService,
            ICacheService cacheService, IMvcForumContext context)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _groupService = GroupService;
            _topicService = topicService;
            _topicTagService = topicTagService;
            _notificationService = notificationService;
        }

        [HttpPost]
        [Authorize]
        public virtual void Subscribe(EmailSubscriptionViewModel subscription)
        {
            if (Request.IsAjaxRequest())
            {
                try
                {
                    // Add logic to add subscr
                    var isGroup = subscription.SubscriptionType.Contains("Group");
                    var isTag = subscription.SubscriptionType.Contains("tag");
                    var id = subscription.Id;
                    var dbUser = MembershipService.GetUser(User.Identity.Name);

                    if (isGroup)
                    {
                        // get the Group
                        var cat = _groupService.Get(id);

                        if (cat != null)
                        {
                            // Create the notification
                            var GroupNotification = new GroupNotification
                            {
                                Group = cat,
                                User = dbUser
                            };
                            //save

                            _notificationService.Add(GroupNotification);
                        }
                    }
                    else if (isTag)
                    {
                        // get the tag
                        var tag = _topicTagService.Get(id);

                        if (tag != null)
                        {
                            // Create the notification
                            var tagNotification = new TagNotification
                            {
                                Tag = tag,
                                User = dbUser
                            };
                            //save

                            _notificationService.Add(tagNotification);
                        }
                    }
                    else
                    {
                        // get the Group
                        var topic = _topicService.Get(id);

                        // check its not null
                        if (topic != null)
                        {
                            // Create the notification
                            var topicNotification = new TopicNotification
                            {
                                Topic = topic,
                                User = dbUser
                            };
                            //save

                            _notificationService.Add(topicNotification);
                        }
                    }

                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Context.RollBack();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            else
            {
                throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }
        }

        [HttpPost]
        [Authorize]
        public virtual void UnSubscribe(EmailSubscriptionViewModel subscription)
        {
            if (Request.IsAjaxRequest())
            {
                try
                {
                    // Add logic to add subscr
                    var isGroup = subscription.SubscriptionType.Contains("Group");
                    var isTag = subscription.SubscriptionType.Contains("tag");
                    var id = subscription.Id;
                    var dbUser = MembershipService.GetUser(User.Identity.Name);
                    if (isGroup)
                    {
                        // get the Group
                        var cat = _groupService.Get(id);

                        if (cat != null)
                        {
                            // get the notifications by user
                            var notifications =
                                _notificationService.GetGroupNotificationsByUserAndGroup(dbUser, cat, true);

                            if (notifications.Any())
                            {
                                foreach (var GroupNotification in notifications)
                                {
                                    // Delete
                                    _notificationService.Delete(GroupNotification);
                                }
                            }
                        }
                    }
                    else if (isTag)
                    {
                        // get the tag
                        var tag = _topicTagService.Get(id);

                        if (tag != null)
                        {
                            // get the notifications by user
                            var notifications =
                                _notificationService.GetTagNotificationsByUserAndTag(dbUser, tag, true);

                            if (notifications.Any())
                            {
                                foreach (var n in notifications)
                                {
                                    // Delete
                                    _notificationService.Delete(n);
                                }
                            }
                        }
                    }
                    else
                    {
                        // get the topic
                        var topic = _topicService.Get(id);

                        if (topic != null)
                        {
                            // get the notifications by user
                            var notifications =
                                _notificationService.GetTopicNotificationsByUserAndTopic(dbUser, topic, true);

                            if (notifications.Any())
                            {
                                foreach (var topicNotification in notifications)
                                {
                                    // Delete
                                    _notificationService.Delete(topicNotification);
                                }
                            }
                        }
                    }

                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Context.RollBack();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            else
            {
                throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }
        }
    }
}
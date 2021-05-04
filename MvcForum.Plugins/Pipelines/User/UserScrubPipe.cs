namespace MvcForum.Plugins.Pipelines.User
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Pipeline;
    using Core.Interfaces.Services;
    using Core.Models.Entities;

    public class UserScrubPipe : IPipe<IPipelineProcess<MembershipUser>>
    {
        private readonly IActivityService _activityService;
        private readonly IGroupService _groupService;
        private readonly IFavouriteService _favouriteService;
        private readonly ILoggingService _loggingService;
        private readonly IMembershipUserPointsService _membershipUserPointsService;
        private readonly INotificationService _notificationService;
        private readonly IPollService _pollService;
        private readonly IPostService _postService;
        private readonly ITopicService _topicService;
        private readonly IVoteService _voteService;

        public UserScrubPipe(IVoteService voteService,
            INotificationService notificationService, 
            IFavouriteService favouriteService, 
            IMembershipUserPointsService membershipUserPointsService,
            IActivityService activityService, 
            IPollService pollService, 
            ITopicService topicService,
            IGroupService groupService, 
            IPostService postService,
            ILoggingService loggingService )
        {
            _voteService = voteService;
            _notificationService = notificationService;
            _favouriteService = favouriteService;
            _membershipUserPointsService = membershipUserPointsService;
            _activityService = activityService;
            _pollService = pollService;
            _topicService = topicService;
            _groupService = groupService;
            _postService = postService;
            _loggingService = loggingService;
        }

        /// <inheritdoc />
        public async Task<IPipelineProcess<MembershipUser>> Process(IPipelineProcess<MembershipUser> input,
            IMvcForumContext context)
        {
            _voteService.RefreshContext(context);
            _notificationService.RefreshContext(context);
            _favouriteService.RefreshContext(context);
            _membershipUserPointsService.RefreshContext(context);
            _activityService.RefreshContext(context);
            _pollService.RefreshContext(context);
            _topicService.RefreshContext(context);
            _groupService.RefreshContext(context);
            _postService.RefreshContext(context);

            try
            {
                // PROFILE
                input.EntityToProcess.Website = string.Empty;
                input.EntityToProcess.Twitter = string.Empty;
                input.EntityToProcess.Facebook = string.Empty;
                input.EntityToProcess.Avatar = string.Empty;
                input.EntityToProcess.Signature = string.Empty;

                // Delete all topics
                var topics = input.EntityToProcess.Topics;
                if (topics != null && topics.Any())
                {
                    var topicList = new List<Topic>();
                    topicList.AddRange(topics);
                    foreach (var topic in topicList)
                    {
                        var topicDeleteResult = await _topicService.Delete(topic);
                        if (!topicDeleteResult.Successful)
                        {
                            input.AddError(topicDeleteResult.ProcessLog.FirstOrDefault());
                            return input;
                        }
                    }
                    input.EntityToProcess.Topics.Clear();
                    await context.SaveChangesAsync();
                }

                // Now sorts Last Posts on topics and delete all the users posts
                var posts = input.EntityToProcess.Posts;
                if (posts != null && posts.Any())
                {
                    var postIds = posts.Select(x => x.Id).ToList();

                    // Get all Groups
                    var allGroups = _groupService.GetAll();

                    // Need to see if any of these are last posts on Topics
                    // If so, need to swap out last post
                    var lastPostTopics = _topicService.GetTopicsByLastPost(postIds, allGroups.ToList());
                    foreach (var topic in lastPostTopics.Where(x => x.User.Id != input.EntityToProcess.Id))
                    {
                        var lastPost = topic.Posts.Where(x => !postIds.Contains(x.Id))
                            .OrderByDescending(x => x.DateCreated)
                            .FirstOrDefault();
                        topic.LastPost = lastPost;
                    }

                    await context.SaveChangesAsync();

                    // Delete all posts
                    var postList = new List<Post>();
                    postList.AddRange(posts);
                    foreach (var post in postList)
                    {
                        // Delete post via pipeline
                        var postDeleteResult = await _postService.Delete(post, true);
                        if (!postDeleteResult.Successful)
                        {
                            input.AddError(postDeleteResult.ProcessLog.FirstOrDefault());
                            return input;
                        }
                    }

                    input.EntityToProcess.UploadedFiles.Clear();
                    input.EntityToProcess.Posts.Clear();

                    await context.SaveChangesAsync();
                }

                // User Votes
                if (input.EntityToProcess.Votes != null)
                {
                    var votesToDelete = new List<Vote>();
                    votesToDelete.AddRange(input.EntityToProcess.Votes);
                    votesToDelete.AddRange(input.EntityToProcess.VotesGiven);
                    foreach (var d in votesToDelete)
                    {
                        _voteService.Delete(d);
                    }
                    input.EntityToProcess.Votes.Clear();
                    input.EntityToProcess.VotesGiven.Clear();
                    await context.SaveChangesAsync();
                }

                // User Group notifications
                if (input.EntityToProcess.GroupNotifications != null)
                {
                    var toDelete = new List<GroupNotification>();
                    toDelete.AddRange(input.EntityToProcess.GroupNotifications);
                    foreach (var obj in toDelete)
                    {
                        _notificationService.Delete(obj);
                    }
                    input.EntityToProcess.GroupNotifications.Clear();
                    await context.SaveChangesAsync();
                }

                // User Favourites
                if (input.EntityToProcess.Favourites != null)
                {
                    var toDelete = new List<Favourite>();
                    toDelete.AddRange(input.EntityToProcess.Favourites);
                    foreach (var obj in toDelete)
                    {
                        _favouriteService.Delete(obj);
                    }
                    input.EntityToProcess.Favourites.Clear();
                    await context.SaveChangesAsync();
                }

                if (input.EntityToProcess.TopicNotifications != null)
                {
                    var notificationsToDelete = new List<TopicNotification>();
                    notificationsToDelete.AddRange(input.EntityToProcess.TopicNotifications);
                    foreach (var topicNotification in notificationsToDelete)
                    {
                        _notificationService.Delete(topicNotification);
                    }
                    input.EntityToProcess.TopicNotifications.Clear();
                }

                // Also clear their points
                var userPoints = input.EntityToProcess.Points;
                if (userPoints.Any())
                {
                    var pointsList = new List<MembershipUserPoints>();
                    pointsList.AddRange(userPoints);
                    foreach (var point in pointsList)
                    {
                        point.User = null;
                        await _membershipUserPointsService.Delete(point);
                    }
                    input.EntityToProcess.Points.Clear();
                }

                // Now clear all activities for this user
                var usersActivities = _activityService.GetDataFieldByGuid(input.EntityToProcess.Id);
                _activityService.Delete(usersActivities.ToList());
                await context.SaveChangesAsync();

                // Also clear their poll votes
                var userPollVotes = input.EntityToProcess.PollVotes;
                if (userPollVotes.Any())
                {
                    var pollList = new List<PollVote>();
                    pollList.AddRange(userPollVotes);
                    foreach (var vote in pollList)
                    {
                        vote.User = null;
                        _pollService.Delete(vote);
                    }
                    input.EntityToProcess.PollVotes.Clear();
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                input.AddError(ex.Message);
                _loggingService.Error(ex);
            }

            return input;
        }
    }
}
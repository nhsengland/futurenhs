namespace MvcForum.Plugins.Pipelines.Group
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

    public class GroupDeletePipe : IPipe<IPipelineProcess<Group>>
    {
        private readonly IGroupPermissionForRoleService _groupPermissionForRoleService;
        private readonly INotificationService _notificationService;
        private readonly ILoggingService _loggingService;
        private readonly ICacheService _cacheService;

        public GroupDeletePipe(IGroupPermissionForRoleService GroupPermissionForRoleService, 
            INotificationService notificationService, ILoggingService loggingService, ICacheService cacheService)
        {
            _groupPermissionForRoleService = GroupPermissionForRoleService;
            _notificationService = notificationService;
            _loggingService = loggingService;
            _cacheService = cacheService;
        }

        /// <inheritdoc />
        public async Task<IPipelineProcess<Group>> Process(IPipelineProcess<Group> input,
            IMvcForumContext context)
        {
            _groupPermissionForRoleService.RefreshContext(context);
            _notificationService.RefreshContext(context);

            try
            {
                // Check if anyone else if using this role
                var okToDelete = !input.EntityToProcess.Topics.Any();

                if (okToDelete)
                {
                    // Get any Grouppermissionforoles and delete these first
                    var rolesToDelete = _groupPermissionForRoleService.GetByGroup(input.EntityToProcess.Id);

                    foreach (var GroupPermissionForRole in rolesToDelete)
                    {
                        _groupPermissionForRoleService.Delete(GroupPermissionForRole);
                    }

                    var GroupNotificationsToDelete = new List<GroupNotification>();
                    GroupNotificationsToDelete.AddRange(input.EntityToProcess.GroupNotifications);
                    foreach (var GroupNotification in GroupNotificationsToDelete)
                    {
                        _notificationService.Delete(GroupNotification);
                    }

                    context.Group.Remove(input.EntityToProcess);

                    await context.SaveChangesAsync();

                    _cacheService.ClearStartsWith("GroupList");
                }
                else
                {
                    input.AddError($"In use by {input.EntityToProcess.Topics} entities");
                    return input;
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
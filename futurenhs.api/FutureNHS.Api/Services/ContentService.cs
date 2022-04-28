using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Repositories.Write.Interfaces;
using FutureNHS.Api.Models.Content;
using FutureNHS.Api.Models.Content.Requests;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace FutureNHS.Api.Services
{
    public class ContentService : IContentService
    {
        private readonly ILogger<ContentService> _logger;
        private readonly IContentCommand _contentCommand;
        private readonly IGroupCommand _groupCommand;
        private readonly ISystemClock _systemClock;

        /// <inheritdoc />
        public ContentService(ILogger<ContentService> logger, IContentCommand contentCommand, ISystemClock systemClock, IGroupCommand groupCommand)
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _contentCommand = contentCommand ?? throw new ArgumentNullException(nameof(contentCommand));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _groupCommand = groupCommand;
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> CreateContentAsync(Guid? userId, Guid groupId, GeneralWebPageCreateRequest createRequest, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (Guid.Empty == groupId) throw new ArgumentOutOfRangeException(nameof(groupId));

            var now = _systemClock.UtcNow.UtcDateTime;

            // If parentId is not null, it's a child to a current site. We update the CMS content only.
            // Else, Its a new group site. Therefore, we create a new db entry and update the content CMS.
            if (!string.IsNullOrEmpty(createRequest.ParentId))
            {
                return await _contentCommand.CreateContentAsync(createRequest, cancellationToken);
            }

            var response = await _contentCommand.CreateContentAsync(createRequest, cancellationToken);

            await _groupCommand.CreateGroupSiteAsync(new GroupSiteDto()
            {
                GroupId = groupId,
                ContentRootId = Guid.Parse(response.Data),
                CreatedAtUTC = now,
                CreatedBy = userId,
                ModifiedBy = null,
                ModifiedAtUTC = null,
            },
                cancellationToken);

            return response;

        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> UpdateContentAsync(Guid? userId, Guid contentId, PageContentModel pageContent, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (Guid.Empty == contentId) throw new ArgumentOutOfRangeException(nameof(contentId));

            return await _contentCommand.UpdateContentAsync(contentId, pageContent, cancellationToken);

        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> DeleteContentAsync(Guid? userId, Guid contentId, int? contentLevel, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (Guid.Empty == contentId) throw new ArgumentOutOfRangeException(nameof(contentId));

            // if content level is 1 it's a root node. In this case we should delete the related database record. 
            if (contentLevel != null && contentLevel == 1)
            {
                await _groupCommand.DeleteGroupSiteAsync(contentId, cancellationToken);
            }

            return await _contentCommand.DeleteContentAsync(contentId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ApiResponse<string>> PublishContentAsync(Guid contentId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == contentId) throw new ArgumentOutOfRangeException(nameof(contentId));

            return await _contentCommand.PublishContentAsync(contentId, cancellationToken);
        }
    }
}

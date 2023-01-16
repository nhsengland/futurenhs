﻿using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.Discussion;
using FutureNHS.Api.Services.Interfaces;
using FutureNHS.Api.Services.Validation;
using Microsoft.AspNetCore.Authentication;
using System.Security;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Database.Read;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;

namespace FutureNHS.Api.Services
{
    public class DiscussionService : IDiscussionService
    {
        private const string DefaultRole = "Standard Members";
        private const string AddDiscussionRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/discussions/add";
        
        private readonly ILogger<DiscussionService> _logger;
        private readonly IDiscussionCommand _discussionCommand;
        private readonly IDiscussionDataProvider _discussionDataProvider;
        private readonly ISystemClock _systemClock;
        private readonly IGroupCommand _groupCommand;
        private readonly IEntityCommand _entityCommand;
        private readonly IPermissionsService _permissionsService;

        public DiscussionService(ISystemClock systemClock, ILogger<DiscussionService> logger, IPermissionsService permissionsService, IDiscussionCommand discussionCommand, IDiscussionDataProvider discussionDataProvider, IGroupCommand groupCommand, IEntityCommand entityCommand)
        {
            _systemClock = systemClock;
            _discussionCommand = discussionCommand;
            _discussionDataProvider = discussionDataProvider;
            _groupCommand = groupCommand;
            _permissionsService = permissionsService;
            _logger = logger;
            _entityCommand = entityCommand;
        }

        public async Task CreateDiscussionAsync(Guid userId, string slug, Discussion discussion, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);

            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: CreateDiscussionAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, groupId.Value, AddDiscussionRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateDiscussionAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var entityId = Guid.NewGuid();

            var discussionDto = new DiscussionDto
            {
                Id = entityId,
                Title = discussion.Title,
                Content = discussion.Content,
                CreatedAtUTC = now,
                CreatedBy = userId,
                IsSticky = discussion.IsSticky,
                IsLocked = false,
                GroupId = groupId.Value
            };

            var validator = new DiscussionValidator();
            var validationResult = await validator.ValidateAsync(discussionDto, cancellationToken);

            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);

            await _discussionCommand.CreateDiscussionAsync(discussionDto, cancellationToken);
        }

        public async Task<IEnumerable<DataAccess.Models.Discussions.Discussion>> GetDiscussionsForGroupAsync(Guid? userId, string slug,
            uint offset, uint limit, string? sortBy, CancellationToken cancellationToken)
        {   
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));
            
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            
            var acceptedSorts = new List<string> {SortingParameters.SortByName, SortingParameters.SortByDateCreated, SortingParameters.SortByLastComment};

            if (!acceptedSorts.Contains(sortBy))
                sortBy = SortingParameters.SortByLastComment;
            
            

            return await _discussionDataProvider.GetDiscussionsForGroupAsync(userId, slug, offset, limit, sortBy,
                cancellationToken);
            

        }
    }
}

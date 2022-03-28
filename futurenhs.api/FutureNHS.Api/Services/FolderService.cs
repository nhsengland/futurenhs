using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.Folder;
using FutureNHS.Api.Services.Interfaces;
using FutureNHS.Api.Services.Validation;
using Microsoft.AspNetCore.Authentication;
using System.Security;

namespace FutureNHS.Api.Services
{
    public class FolderService : IFolderService
    {
        private const string DefaultRole = "Standard Members";
        private const string AddFolderRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/folders/add";
        private const string EditFolderRole = $"https://schema.collaborate.future.nhs.uk/groups/v1/folders/edit";

        private readonly ILogger<DiscussionService> _logger;
        private readonly IFolderCommand _folderCommand;
        private readonly IGroupCommand _groupCommand;
        private readonly ISystemClock _systemClock;
        private readonly IPermissionsService _permissionsService;

        public FolderService(ISystemClock systemClock, ILogger<DiscussionService> logger, IPermissionsService permissionsService, 
            IFolderCommand folderCommand, IGroupCommand groupCommand)
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _folderCommand = folderCommand ?? throw new ArgumentNullException(nameof(folderCommand));
            _groupCommand = groupCommand ?? throw new ArgumentNullException(nameof(groupCommand));
            _permissionsService = permissionsService ?? throw new ArgumentNullException(nameof(permissionsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> CreateFolderAsync(Guid userId, string slug, Folder folder, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));


            var now = _systemClock.UtcNow.UtcDateTime;

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);

            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: CreateFolderAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, groupId.Value, AddFolderRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateFolderAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }

            var folderDto = new FolderDto()
            {
                Title = folder.Title,
                Description = folder.Description,
                CreatedAtUTC = now,
                CreatedBy = userId,
                ModifiedBy = null,
                ModifiedAtUTC = null,
                ParentFolder = null,
                GroupId = groupId.Value,
                IsDeleted = false
            };

            var validator = new FolderValidator(_folderCommand);
            var validationResult = await validator.ValidateAsync(folderDto, cancellationToken);

            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);

            return await _folderCommand.CreateFolderAsync(userId, groupId.Value, folderDto, cancellationToken);
        }

        public async Task<Guid> CreateChildFolderAsync(Guid userId, string slug, Guid parentFolderId, Folder folder, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);

            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: CreateChildFolderAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var userCanPerformAction = await _permissionsService.UserCanPerformActionAsync(userId, groupId.Value, AddFolderRole, cancellationToken);

            if (!userCanPerformAction)
            {
                _logger.LogError($"Error: CreateChildFolderAsync - User:{0} does not have access to group:{1}", userId, slug);
                throw new SecurityException($"Error: User does not have access");
            }            

            var folderDto = new FolderDto()
            {
                Title = folder.Title,
                Description = folder.Description,
                CreatedAtUTC = now,
                CreatedBy = userId,
                ModifiedBy = null,
                ModifiedAtUTC = null,
                ParentFolder = parentFolderId,
                GroupId = groupId.Value,
                IsDeleted = false
            };

            var validator = new FolderValidator(_folderCommand);
            var validationResult = await validator.ValidateAsync(folderDto, cancellationToken);

            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);

            return await _folderCommand.CreateFolderAsync(userId, groupId.Value, folderDto, cancellationToken);
        }

        public async Task UpdateFolderAsync(Guid userId, string slug, Guid folderId, Folder folder, byte[] rowVersion, CancellationToken cancellationToken)
        {
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (Guid.Empty == folderId) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrEmpty(slug)) throw new ArgumentOutOfRangeException(nameof(slug));

            var now = _systemClock.UtcNow.UtcDateTime;

            var groupId = await _groupCommand.GetGroupIdForSlugAsync(slug, cancellationToken);
            if (!groupId.HasValue)
            {
                _logger.LogError($"Error: UpdateFolderAsync - Group not found for slug:{0}", slug);
                throw new KeyNotFoundException("Error: Group not found for slug");
            }

            var userCanEditFolder = await _permissionsService.UserCanPerformActionAsync(userId, slug, EditFolderRole, cancellationToken);
            var databaseFolderDto = await _folderCommand.GetFolderAsync(folderId, cancellationToken);
            if (databaseFolderDto.CreatedBy != userId && !userCanEditFolder)
            {
                _logger.LogError($"Forbidden: UpdateFolderAsync - User:{0} does not have permission to edit folder:{1}", userId, folderId);
                throw new ForbiddenException("Forbidden: User does not have permission to edit this folder");
            }

            if (!databaseFolderDto.RowVersion.SequenceEqual(rowVersion))
            {
                _logger.LogError($"Precondition Failed: UpdateFolderAsync - Folder:{0} has changed prior to submission ", folderId);
                throw new PreconditionFailedExeption("Precondition Failed: Folder has changed prior to submission");
            }

            var folderDto = new FolderDto()
            {
                Id = folderId,
                Title = folder.Title,
                Description = folder.Description,
                ModifiedBy = userId,
                ModifiedAtUTC = now,
                ParentFolder = databaseFolderDto.ParentFolder,
                GroupId = groupId.Value,
            };

            var validator = new FolderValidator(_folderCommand);
            var validationResult = await validator.ValidateAsync(folderDto, cancellationToken);

            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);


            await _folderCommand.UpdateFolderAsync(userId, folderDto, rowVersion, cancellationToken);
        }
    }
}

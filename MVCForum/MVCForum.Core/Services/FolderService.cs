using MvcForum.Core.Interfaces;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models.FilesAndFolders;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Models;
using MvcForum.Core.Repositories.Repository.Interfaces;
using MvcForum.Web.ViewModels.Folder;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Services
{
    public sealed class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFolderCommand _folderCommand;
        private readonly IFeatureManager _featureManager;

        public FolderService(IFolderRepository folderRepository, IFeatureManager featureManager, IFolderCommand folderCommand, IFileRepository fileRepository)
        {
            _folderRepository = folderRepository ?? throw new ArgumentNullException(nameof(folderRepository));
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
            _folderCommand = folderCommand ?? throw new ArgumentNullException(nameof(folderCommand));
            _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
        }
        
        public async Task<FolderViewModel> GetFolderAsync(string groupSlug, Guid? folderId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(groupSlug))
            {
                throw new ArgumentNullException(nameof(groupSlug));
            }

            if (!folderId.HasValue)
            {
                return new FolderViewModel
                {
                    Slug = groupSlug,
                    Folder = null,
                    ChildFolders = await _folderRepository.GetRootFoldersForGroupAsync(groupSlug: groupSlug, cancellationToken: cancellationToken),
                    Files = new List<FileReadViewModel>()
                };
            }
            else
            {
                return new FolderViewModel
                {
                    Slug = groupSlug,
                    Folder = await _folderRepository.GetFolderAsync(folderId.Value, cancellationToken),
                    ChildFolders = await _folderRepository.GetChildFoldersForFolderAsync(parentFolderId: folderId.Value, cancellationToken: cancellationToken),
                    Files = (await _fileRepository.GetFilesAsync(folderId.Value, cancellationToken: cancellationToken)).ToList()
                };
            }
        }

        public Task<bool> IsFolderNameValidAsync(Guid? folderId, string folderName, Guid? parentFolderId,
                                                 Guid parentGroupId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == folderId) throw new ArgumentOutOfRangeException(nameof(folderId));
            if (string.IsNullOrWhiteSpace(folderName)) throw new ArgumentNullException(nameof(folderName));
            if (Guid.Empty == parentGroupId) throw new ArgumentOutOfRangeException(nameof(parentGroupId));

            return _folderRepository.IsFolderNameValidAsync(folderId, folderName, parentFolderId, parentGroupId, cancellationToken);
        }

        public Task<bool> IsFolderIdValidAsync(Guid folderId,
                                               CancellationToken cancellationToken)
        {
            if (Guid.Empty == folderId)
            {
                throw new ArgumentOutOfRangeException(nameof(folderId));
            }

            return _folderRepository.IsFolderIdValidAsync(folderId, cancellationToken);
        }

        public Guid CreateFolder(FolderWriteViewModel model)
        {
            return _folderCommand.CreateFolder(model);
        }

        public void UpdateFolder(FolderWriteViewModel model)
        {
            if (model.FolderId != null) _folderCommand.UpdateFolder(model);
        }

        public Task<IEnumerable<BreadCrumbItem>> GenerateBreadcrumbTrailAsync(Guid folderId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == folderId)
            {
                throw new ArgumentOutOfRangeException(nameof(folderId));
            }

            return _folderRepository.GenerateBreadcrumbTrailAsync(folderId, cancellationToken);
        }
     
        public async Task<bool> IsUserAdminAsync(string groupSlug, Guid userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(groupSlug))
            {
                throw new ArgumentNullException(nameof(groupSlug));
            }
            
            if (Guid.Empty == userId)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            return await _folderRepository.IsUserAdminAsync(groupSlug, userId, cancellationToken);
        }

		public Task<bool> UserHasGroupAccessAsync(string groupSlug, Guid userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(groupSlug)) return Task.FromResult(false);
            if (Guid.Empty == userId) return Task.FromResult(false);

            return _folderRepository.UserHasGroupAccessAsync(groupSlug, userId, cancellationToken);
        }

        public Task<bool> UserHasFileAccessAsync(Guid fileId, Guid userId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == fileId) return Task.FromResult(false);
            if (Guid.Empty == userId) return Task.FromResult(false);

            return _folderRepository.UserHasFileAccessAsync(fileId, userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteFolderAsync(Guid folderId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (folderId == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(folderId));

            try
            {
                return await _folderCommand.DeleteFolderAsync(folderId, cancellationToken);
            } catch (SqlException)
            {
                // TODO: Implement logging to improve debugging - returning false here to surface the error message as defined in the AC.
                return false;
            }
        }

    }
}

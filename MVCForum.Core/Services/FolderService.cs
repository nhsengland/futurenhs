using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models.FilesAndFolders;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Models;
using MvcForum.Core.Repositories.Repository.Interfaces;
using MvcForum.Web.ViewModels.Folder;

namespace MvcForum.Core.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFolderCommand _folderCommand;
        private readonly IFeatureManager _featureManager;

        public FolderService(IFolderRepository folderRepository, IFeatureManager featureManager, IFolderCommand folderCommand, IFileRepository fileRepository)
        {
            _folderRepository = folderRepository;
            _featureManager = featureManager;
            _folderCommand = folderCommand;
            _fileRepository = fileRepository;
        }
        
        public FolderViewModel GetFolder(string groupSlug, Guid? folderId)
        {
            FolderViewModel model;

            if (!folderId.HasValue)
            {
                model = new FolderViewModel
                {
                    Slug = groupSlug,
                    Folder = null,
                    ChildFolders = _folderRepository.GetRootFoldersForGroup(groupSlug),
                    Files = new List<FileReadViewModel>()
                };
            }
            else
            {
                model = new FolderViewModel
                {
                    Slug = groupSlug,
                    Folder = _folderRepository.GetFolder(folderId.Value),
                    ChildFolders = _folderRepository.GetChildFoldersForFolder(folderId.Value),
                    Files = _fileRepository.GetFiles(folderId.Value)
                };
            }

            return model;
        }

        /// <summary>
        /// Get folder by Id, folder name and parent - validate folder exists for create/update, i.e. no duplicate names allowed.
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="folderName"></param>
        /// <param name="parentFolder"></param>
        /// <returns></returns>
        public FolderReadViewModel GetFolder(Guid? folderId, string folderName, Guid? parentFolder)
        {
            return _folderRepository.GetFolder(folderId, folderName, parentFolder);
        }

        public Guid CreateFolder(FolderWriteViewModel model)
        {
            return _folderCommand.CreateFolder(model);
        }

        public void UpdateFolder(FolderWriteViewModel model)
        {
            if (model.FolderId != null) _folderCommand.UpdateFolder(model);
        }

        public IEnumerable<BreadCrumbItem> GenerateBreadcrumbTrail(Guid folderId)
        {
            return _folderRepository.GenerateBreadcrumbTrail(folderId);
        }

     
        public bool UserIsAdmin(string groupSlug, Guid? userId)
        {
            if (string.IsNullOrWhiteSpace(groupSlug))
                return false;

            return userId.HasValue && _folderRepository.UserIsAdmin(groupSlug, userId.Value);
        }
    }
}

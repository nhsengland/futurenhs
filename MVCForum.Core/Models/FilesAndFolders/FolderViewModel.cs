//-----------------------------------------------------------------------
// <copyright file="FolderListViewModel.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Web.ViewModels.Folder
{
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Ioc;
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;
    using Unity;

    public class FolderViewModel
    {
        public FolderReadViewModel Folder { get; set; }
        public List<FolderReadViewModel> ChildFolders { get; set; }
        public List<FileReadViewModel> Files { get; set; }
        public IEnumerable<BreadCrumbItem> BreadCrumbTrail { get; set; }
        public string Slug { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }
        public Guid GroupId { get; set; }

        /// <summary>
        /// Get the full name for a user Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetFullNameForUserId(Guid userId)
        {
            // TODO - review this, not overly happy with it...
            var user = UnityHelper.Container.Resolve<IMembershipService>().Get(userId);
            return user != null ? $"{user.FirstName} {user.Surname}" : "unknown";
        }
    }
}
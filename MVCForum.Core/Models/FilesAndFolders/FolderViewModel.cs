//-----------------------------------------------------------------------
// <copyright file="FolderListViewModel.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Web.ViewModels.Folder
{
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;

    public class FolderViewModel
    {
        public FolderReadViewModel Folder { get; set; }
        public List<FolderReadViewModel> ChildFolders { get; set; }
        public IEnumerable<BreadCrumbItem> BreadCrumbTrail { get; set; }
        public string Slug { get; set; }
        public bool IsAdmin { get; set; }
        public Guid GroupId { get; set; }
    }
}
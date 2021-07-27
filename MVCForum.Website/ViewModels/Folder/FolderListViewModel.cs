//-----------------------------------------------------------------------
// <copyright file="FolderListViewModel.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Web.ViewModels.Folder
{
    using MvcForum.Core.Repositories.Models;
    using System.Collections.Generic;

    public class FolderListViewModel
    {
        public string Slug { get; set; }
        public FolderReadViewModel Folder { get; set; }
        public List<FolderReadViewModel> ChildFolders { get; set; }
        public bool IsAdmin { get; set; }

        public List<File> Files { get; set; }
    }
}

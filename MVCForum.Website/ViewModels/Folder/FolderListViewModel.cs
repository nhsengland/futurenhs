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
    using System.Linq;
    using System.Web;

    public class FolderListViewModel
    {
        public string Slug { get; set; }
        public Folder Folder { get; set; }
        public List<Folder> ChildFolders { get; set; }

        public List<File> Files { get; set; }
    }
}
namespace MvcForum.Web.ViewModels.Folder
{
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Ioc;
    using MvcForum.Core.Models.Enums;
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;
    using Unity;

    public class FolderViewModel
    {
        public FolderReadViewModel Folder { get; set; }
        public List<FolderReadViewModel> ChildFolders { get; set; }
        public List<FileReadViewModel> Files { get; set; }
        public BreadcrumbsViewModel Breadcrumbs { get; set; }
        public string Slug { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }
        public Guid GroupId { get; set; }
        public GroupUserStatus GroupUserStatus { get; set; }
        public bool IsMember { get; set; }
        public bool HasError { get; set; }
    }
}
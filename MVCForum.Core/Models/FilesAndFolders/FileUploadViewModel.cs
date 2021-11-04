namespace MvcForum.Core.Models.FilesAndFolders
{
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class FileUploadViewModel
    {
        public string FolderName { get; set; }

        public BreadcrumbsViewModel Breadcrumbs { get; set; }

        public FileWriteViewModel FileToUpload { get; set; }
    }
}

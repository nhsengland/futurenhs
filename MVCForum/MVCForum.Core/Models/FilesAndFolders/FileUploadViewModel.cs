namespace MvcForum.Core.Models.FilesAndFolders
{
    using MvcForum.Core.Repositories.Models;

    public sealed class FileUploadViewModel
    {
        public string FolderName { get; set; }
        public BreadcrumbsViewModel Breadcrumbs { get; set; }
        public FileWriteViewModel FileToUpload { get; set; }
    }
}

namespace MvcForum.Core.Interfaces.Providers
{
    using System.IO;
    using System.Web;

    public interface IStorageProvider
    {
        string GetUploadFolderPath(bool createIfNotExist, params object[] subFolders);

        string BuildFileUrl(params object[] subPath);

        string SaveAs(string uploadFolderPath, string fileName, Stream file);
    }
}
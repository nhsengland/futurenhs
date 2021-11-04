namespace MvcForum.Core.Interfaces.Helpers
{
    using System.IO;

    public interface IValidateFileType
    {
        bool ContentMatchesExtension(Stream fileStream, string fileExtension);
    }
}

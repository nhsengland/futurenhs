namespace FutureNHS.Api.Helpers.Interfaces
{
    public interface IFileTypeValidator
    {
        bool ContentMatchesExtension(Stream fileStream, string fileExtension);
    }
}

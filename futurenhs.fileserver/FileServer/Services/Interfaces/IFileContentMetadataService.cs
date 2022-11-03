using FileServer.Models;

namespace FileServer.Services.Interfaces;

public interface IFileContentMetadataService
{
    /// <summary>
    /// Tasked with retrieving a file located in storage and writing it into <paramref name="streamToWriteTo"/>
    /// </summary>
    /// <param name="fileMetadata">The metadata pertinent to the file whose contents we are going to try and write to the stream</param>
    /// <param name="streamToWriteTo">The stream to which the content of the file will be written in the success case/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Information on the content written to the stream</returns>
    Task<FileContentMetadata> GetDetailsAndPutContentIntoStreamAsync(UserFileMetadata fileMetadata, Stream streamToWriteTo, CancellationToken cancellationToken);

    Task<string?> SaveFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken);
}
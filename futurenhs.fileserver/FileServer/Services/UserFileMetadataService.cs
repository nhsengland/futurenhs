using FileServer.DataAccess.Interfaces;
using FileServer.Models;
using FileServer.Services.Interfaces;
using File = FileServer.Models.File;

namespace FileServer.Services;

public sealed class UserFileMetadataService : IUserFileMetadataService
{
    private readonly ILogger<UserFileMetadataService>? _logger;
    private readonly IFileMetaDataProvider _fileMetaDataProvider;

    public UserFileMetadataService(IFileMetaDataProvider fileMetaDataProvider, ILogger<UserFileMetadataService>? logger)
    {
        _logger = logger;
        _fileMetaDataProvider = fileMetaDataProvider;
    }

    public async Task<UserFileMetadata?> GetForFileAsync(Guid fileId, AuthenticatedUser authenticatedUser, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var metaData = await _fileMetaDataProvider.GetFileMetaDataForUserAsync(fileId, authenticatedUser.Id, cancellationToken);
        
        if(metaData == null)
            metaData =  await _fileMetaDataProvider.GetFileVersionMetaDataForUserAsync(fileId, authenticatedUser.Id, cancellationToken);
        
        return metaData;
    }
}
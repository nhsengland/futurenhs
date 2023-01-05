using System.Security.Authentication;
using FileServer.DataAccess.Interfaces;
using FileServer.Enums;
using FileServer.Models;
using FileServer.Services.Interfaces;
using FileServer.Wopi.Interfaces;

namespace FileServer.Wopi.Services
{
    internal sealed class WopiFileContentService : IWopiFileContentService
    {

        private readonly IUserFileMetadataService _userFileMetadataService;
        private readonly IFileContentMetadataService _fileMetaDataProvider;

        public WopiFileContentService(IUserFileMetadataService userFileMetadataService, IFileContentMetadataService fileMetaDataProvider)
        {
            _userFileMetadataService = userFileMetadataService;
            _fileMetaDataProvider = fileMetaDataProvider;
        }
        
        public async Task<FileContentMetadata> GetFileContentAsync(Models.File file, AuthenticatedUser authenticatedUser,Stream responseStream, CancellationToken cancellationToken)
        {
            var fileMetadata = await _userFileMetadataService.GetForFileAsync(file, authenticatedUser, cancellationToken);

            if (fileMetadata is null) throw new FileNotFoundException("Could not find the file requested");
            if (authenticatedUser.UserAccess is not (FileAccessPermission.Edit or FileAccessPermission.View)) throw new AuthenticationException("User does not have permission to view this file");
            var fileContentMetadataRepository = _fileMetaDataProvider;

            var fileContentMetadata = await fileContentMetadataRepository.GetDetailsAndPutContentIntoStreamAsync(fileMetadata, responseStream, cancellationToken);

            if (fileContentMetadata.IsEmpty) throw new ApplicationException("Unable to pull the content for the requested file version");
            if(file.Version is not null)
                if (!string.Equals(fileContentMetadata.ContentVersion, file.Version, StringComparison.OrdinalIgnoreCase)) throw new ApplicationException("The blob store client returned a version of the blob that does not match the version requested");
            
            return fileContentMetadata;
        }
        
        public async Task<AzureBlobMetadata> SaveFileAsync(Stream stream,string fileName,string contentType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _fileMetaDataProvider.SaveFileAsync(stream, fileName, contentType, cancellationToken);
        }
    }
}

namespace MvcForum.Core.Repositories.Repository
{
    using Dapper;
    using MvcForum.Core.Models.Enums;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Implements the <see cref="IFileRepository"/> for interactions with uploaded file.
    /// </summary>
    public class FileRepository : IFileRepository
    {
        /// <summary>
        /// Instance of the <see cref="IDbConnectionFactory"/> to create db connections.
        /// </summary>
        private readonly IDbConnectionFactory _connectionFactory;

        /// <summary>
        /// Constructs a new instance of the <see cref="FileRepository"/>.
        /// </summary>
        /// <param name="connectionFactory">The <see cref="IDbConnectionFactory"/> to create db connections.</param>
        public FileRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Method to get a file by Id.
        /// </summary>
        /// <param name="fileId">Id of the <see cref="FileReadViewModel"/>.</param>
        /// <returns>The requested <see cref="FileReadViewModel"/>.</returns>
        public async Task<FileReadViewModel> GetFileAsync(Guid fileId, CancellationToken cancellationToken)
        {
            try
            {
                var dbConnection = _connectionFactory.CreateReadOnlyConnection();
                var query = @"SELECT f.Id, 
                                     f.Title, 
                                     f.Description, 
                                     f.FileName, 
                                     f.BlobName,
                                     f.CreatedAtUtc,
                                     f.ModifiedAtUtc,
                                     f.CreatedBy,
                                     f.ParentFolder,
									 f.FileStatus AS Status,
                                     m.FirstName + ' ' + m.Surname AS UserName,
                                     m.Slug AS UserSlug,
									 CASE 
										WHEN f.ModifiedAtUtc IS NULL 
										THEN f.CreatedAtUtc
										ELSE f.ModifiedAtUtc
									 END AS LastModifiedAtUtc,
									 CASE
										WHEN f.ModifiedBy IS NULL
										THEN (SELECT FirstName + ' ' + Surname FROM MembershipUser WHERE Id = CreatedBy)
										ELSE (SELECT FirstName + ' ' + Surname FROM MembershipUser WHERE Id = ModifiedBy)
									 END AS ModifiedUserName,
									 CASE
										WHEN f.ModifiedBy IS NULL
										THEN (SELECT Slug FROM MembershipUser WHERE Id = CreatedBy)
										ELSE (SELECT Slug FROM MembershipUser WHERE Id = ModifiedBy)
									 END AS ModifiedUserSlug
                            FROM [File] f 
                            JOIN MembershipUser m ON m.Id = f.CreatedBy 
                            WHERE f.Id = @fileId";

                var commandDefinition = new CommandDefinition(query, new { fileId }, cancellationToken: cancellationToken);

                return (await dbConnection.QueryAsync<FileReadViewModel>(commandDefinition)).SingleOrDefault();
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// Gets all files for a given folder Id.
        /// </summary>
        /// <param name="folderId">Id of the parent folder.</param>
        /// <returns>List of files <see cref="List{File}"/>.</returns>
        public async Task<IEnumerable<FileReadViewModel>> GetFilesAsync(Guid folderId, UploadStatus status = UploadStatus.Uploaded, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var dbConnection = _connectionFactory.CreateReadOnlyConnection();

                var query = @"SELECT f.Id, 
                                     f.Title, 
                                     f.CreatedAtUtc, 
                                     f.CreatedBy, 
                                     m.FirstName + ' ' + m.Surname AS UserName,
                                     m.Slug AS UserSlug 
                            FROM [File] f 
                            JOIN MembershipUser m ON m.Id = f.CreatedBy 
                            WHERE f.ParentFolder = @folderId
                            AND f.FileStatus = @fileStatus
                            ORDER BY f.Title";

                var commandDefinition = new CommandDefinition(query, new { folderId = folderId, fileStatus = status }, cancellationToken: cancellationToken);

                return (await dbConnection.QueryAsync<FileReadViewModel>(commandDefinition));
            }
            catch (Exception) { }
            return null;
        }
    }
}

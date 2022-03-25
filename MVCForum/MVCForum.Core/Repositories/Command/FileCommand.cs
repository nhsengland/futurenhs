namespace MvcForum.Core.Repositories.Command
{
    using MvcForum.Core.Data.Context;
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using System;
    using MvcForum.Core.Models.Entities;
    using Status = MvcForum.Core.Models.Enums.UploadStatus;
    using MvcForum.Core.Models.FilesAndFolders;
    using System.Threading.Tasks;
    using System.Threading;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using Dapper;

    /// <summary>
    /// Implements the <see cref="IFileCommand"/> to process write operations of <see cref="File"/>.
    /// </summary>
    public class FileCommand : IFileCommand
    {
        /// <summary>
        /// Instance of the <see cref="MvcForumContext"/>.
        /// </summary>
        private IMvcForumContext _context { get; set; }

        private readonly IDbConnectionFactory _connectionFactory;

        /// <summary>
        /// Constructs a new instance ofthe <see cref="FileCommand"/> setting the db context.
        /// </summary>
        /// <param name="context">Instance of <see cref=""/></param>
        public FileCommand(IMvcForumContext context, IDbConnectionFactory connectionFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// Method to add a <see cref="File"/> to the database.
        /// </summary>
        /// <param name="file">The <see cref="File"/> to add.</param>
        /// <returns>The file Id.</returns>
        public Guid Create(FileWriteViewModel file)
        {
            var fileCreate = new File
            {
                FileName = file.Name,
                Title = file.Name,
                Description = file.Description,
                CreatedBy = (Guid)file.CreatedBy,
                CreatedAtUtc = DateTime.UtcNow,
                ParentFolder = file.FolderId,
                FileStatus = (int)Status.Uploading,
            };

            var createdFile = _context.Files.Add(fileCreate);
            _context.SaveChanges();
            return createdFile.Id;
        }

        /// <summary>
        /// Method to update a <see cref="File"/>.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Guid Update(FileWriteViewModel file)
        {
            var dbFile = _context.Files.Find(file.FileId);
            if (dbFile != null)
            {
                dbFile.Title = file.Name;
                dbFile.Description = file.Description;
                dbFile.ModifiedBy = file.ModifiedBy;
                dbFile.ModifiedAtUtc = DateTime.UtcNow;
                dbFile.FileStatus = file.UploadStatus;
                dbFile.FileName = file.FileName;
                dbFile.FileExtension = file.FileExtension;
                dbFile.FileSizeBytes = file.FileSize;
                dbFile.BlobName = file.FileUrl;
                dbFile.BlobHash = file.BlobHash;
            }

            _context.SaveChanges();
            return file.FileId;
        }

        public void Delete(FileWriteViewModel file)
        {
            var dbFile = _context.Files.Find(file.FileId);
            dbFile.FileStatus = (int)Status.Recycled;
            _context.SaveChanges();
        }

        public async Task<bool> UpdateAsync(FileUpdateViewModel model, CancellationToken cancellationToken = default)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            var sql = @"UPDATE [dbo].[File] 
                        SET [Title] = @title, 
                            [Description] = @description,
                            [ModifiedBy] = @modifiedBy,
                            [ModifiedAtUtc] = GETUTCDATE()
                        WHERE [Id] = @fileId";

            var commandDefinition = new CommandDefinition(sql, new
            {
                fileId = model.FileId,
                title = model.Name,
                description = model.Description,
                modifiedBy = model.ModifiedBy
            }, cancellationToken: cancellationToken);

            using (var conn = _connectionFactory.CreateWriteOnlyConnection())
            {
                if (await conn.ExecuteAsync(commandDefinition) == 1)
                {
                    return true;
                }

                return false;
            }
        }
    }
}

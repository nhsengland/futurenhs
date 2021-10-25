using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Models.Entities;
using MvcForum.Core.Models.FilesAndFolders;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;

namespace MvcForum.Core.Repositories.Command
{
    public class FolderCommand : IFolderCommand
    {
        private readonly IMvcForumContext _context;
        private readonly IDbConnectionFactory _connectionFactory;

        public FolderCommand(IMvcForumContext context, IDbConnectionFactory connectionFactory)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (connectionFactory is null)
                throw new ArgumentNullException(nameof(connectionFactory));
            
            _context = context;
            _connectionFactory = connectionFactory;
        }

        public Guid CreateFolder(FolderWriteViewModel folder)
        {
            var newFolder = new Folder
            {
                Name = folder.FolderName,
                Description = folder.Description,
                AddedBy = folder.AddedBy,
                ParentFolder = folder.ParentFolder,
                CreatedAtUtc = DateTime.UtcNow,
                ParentGroup = folder.ParentGroup
            };


            _context.Folder.Add(newFolder);
            _context.SaveChanges();
            return newFolder.Id;
        }

        public void UpdateFolder(FolderWriteViewModel folder)
        {
            var result = _context.Folder.FirstOrDefault(x => x.Id == folder.FolderId);

            if (result != null)
            {
                if (result.Name != folder.FolderName
                    || result.IsDeleted != folder.IsDeleted
                    || result.Description != folder.Description)
                {
                    result.Name = folder.FolderName;
                    result.Description = folder.Description;
                    result.IsDeleted = folder.IsDeleted;
                    _context.SaveChanges();
                }
            }
        }

        /// <inheritdoc />
        public Task<bool> DeleteFolderAsync(Guid folderId, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var dbConnection = _connectionFactory.CreateWriteOnlyConnection())
            {
                return dbConnection.QuerySingleAsync<bool>("usp_delete_folder", new { FolderId = folderId }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}

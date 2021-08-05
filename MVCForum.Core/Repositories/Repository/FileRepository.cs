//-----------------------------------------------------------------------
// <copyright file="FileRepository.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace MvcForum.Core.Repositories.Repository
{
    using Dapper;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        public FileReadViewModel GetFile(Guid fileId)
        {
            try
            {
                var conn = _connectionFactory.CreateReadOnlyConnection();
                var query = @"SELECT * FROM [File] WHERE Id = @fileId";
                return conn.Query<FileReadViewModel>(query, new { fileId = fileId }).FirstOrDefault();
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// Gets all files for a given folder Id.
        /// </summary>
        /// <param name="folderId">Id of the parent folder.</param>
        /// <returns>List of files <see cref="List{File}"/>.</returns>
        public List<FileReadViewModel> GetFiles(Guid folderId)
        {
            try
            {
                var conn = _connectionFactory.CreateReadOnlyConnection();
                var query = @"SELECT * FROM [File] WHERE ParentFolder = @folderId";
                return conn.Query<FileReadViewModel>(query, new { folderId = folderId }).ToList();
            }
            catch (Exception) { }
            return null;
        }
    }
}

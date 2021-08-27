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
                var query = @"SELECT f.Id, 
                                     f.Title, 
                                     f.Description, 
                                     f.FileName, 
                                     f.FileUrl,
                                     f.CreatedAtUtc,
                                     f.ModifiedAtUtc,
                                     f.CreatedBy,
                                     f.ParentFolder,
									 f.UploadStatus AS Status,
                                     m.FirstName + ' ' + m.Surname AS UserName,
                                     m.Slug AS UserSlug,
                                     mu.FirstName + ' ' + mu.Surname as ModifiedUserName, 
                                     mu.Slug AS ModifiedUserSlug 
                            FROM [File] f 
                            JOIN MembershipUser m ON m.Id = f.CreatedBy 
                            LEFT JOIN MembershipUser mu ON m.Id = f.ModifiedBy  
                            WHERE f.Id = @fileId";
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
                var query = @"SELECT f.Id, 
                                     f.Title, 
                                     f.CreatedAtUtc, 
                                     f.CreatedBy, 
                                     m.FirstName + ' ' + m.Surname AS UserName,
                                     m.Slug AS UserSlug 
                            FROM [File] f 
                            JOIN MembershipUser m ON m.Id = f.CreatedBy 
                            WHERE f.ParentFolder = @folderId
                            ORDER BY f.Title";
                return conn.Query<FileReadViewModel>(query, new { folderId = folderId }).ToList();
            }
            catch (Exception) { }
            return null;
        }
    }
}

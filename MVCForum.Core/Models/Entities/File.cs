//-----------------------------------------------------------------------
// <copyright file="File.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace MvcForum.Core.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using MvcForum.Core.Utilities;

    /// <summary>
    /// Defines the file entity to store file meta-data.
    /// </summary>
    [Table("File")]
    public class File
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="File"/> class.
        /// </summary>
        public File()
        {
            this.Id = GuidComb.GenerateComb();
        }

        /// <summary>
        /// Gets or sets the file id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the file title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the parent folder.
        /// </summary>
        public Guid ParentFolder { get; set; }

        /// <summary>
        /// Gets or sets the file description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file size.
        /// </summary>
        public long FileSizeBytes { get; set; }
        
        /// <summary>
        /// Gets or sets the file extension.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the file url.
        /// </summary>
        public string BlobName { get; set; }

        /// <summary>
        /// Gets or sets the blob hash (md5)
        /// </summary>
        public byte[] BlobHash { get; set; }

        /// <summary>
        /// Gets or sets the file created by.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the file modified by.
        /// </summary>
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the file created date.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the file modified date.
        /// </summary>
        public DateTime? ModifiedAtUtc { get; set; }
        
        /// <summary>
        /// Gets or sets the upload status of the file.
        /// </summary>
        public int FileStatus { get; internal set; }
    }
}

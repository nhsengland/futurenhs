//-----------------------------------------------------------------------
// <copyright file="Folder.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Models
{
    using System;

    /// <summary>
    /// Represents a folder object.
    /// </summary>
    public class Folder
    {
        /// <summary>
        /// Gets or sets the folder Id.
        /// </summary>
        public Guid FolderId { get; set; }

        /// <summary>
        /// Gets or sets the folder name.
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// Gets or sets the folder description.
        /// </summary>
        public string Description { get; set; }

        public int FileCount { get; set; }

        /// <summary>
        /// Gets or sets who added the folder.
        /// </summary>
        public Guid AddedBy { get; set; }

        /// <summary>
        /// Gets or sets when the Folder was added.
        /// </summary>
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// Gets or sets the parent of the folder, may not have a parent i.e. root.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the status of the folder.
        /// </summary>
        public int Status { get; set; }
    }
}

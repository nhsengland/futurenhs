//-----------------------------------------------------------------------
// <copyright file="Folder.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a Folder entity.
    /// </summary>
    [Table("Folder")]
    public partial class Folder
    {
        public Folder()
        {
            Folder1 = new HashSet<Folder>();
        }

        /// <summary>
        /// Gets or sets the folder Id.
        /// </summary>
        public Guid FolderId { get; set; }

        /// <summary>
        /// Gets or sets the folder name.
        /// </summary>
        [Required]
        [StringLength(512)]
        public string FolderName { get; set; }

        /// <summary>
        /// Gets or sets the folder description.
        /// </summary>
        public string Description { get; set; }

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

        public virtual ICollection<Folder> Folder1 { get; set; }

        public virtual Folder Folder2 { get; set; }
    }
}

//-----------------------------------------------------------------------
// <copyright file="Context.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Database
{
    using MvcForum.Core.Repositories.Database.Models;
    using System.Data.Entity;

    /// <summary>
    /// DbContext
    /// </summary>
    public partial class Context : DbContext
    {
        public Context() : base("name=Context")
        {
        }

        public virtual DbSet<Folder> Folders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Folder>()
                .HasMany(e => e.Folder1)
                .WithOptional(e => e.Folder2)
                .HasForeignKey(e => e.ParentId);
        }
    }
}

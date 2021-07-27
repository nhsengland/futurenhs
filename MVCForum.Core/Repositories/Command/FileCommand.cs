namespace MvcForum.Core.Repositories.Command
{
    using MvcForum.Core.Data.Context;
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using System;
    using MvcForum.Core.Models.Entities;
    using System.Data.Entity;
    using Status = MvcForum.Core.Models.Enums.UploadStatus;

    /// <summary>
    /// Implements the <see cref="IFileCommand"/> to process write operations of <see cref="File"/>.
    /// </summary>
    public class FileCommand : IFileCommand
    {
        /// <summary>
        /// Instance of the <see cref="MvcForumContext"/>.
        /// </summary>
        private IMvcForumContext _context { get; set; }

        /// <summary>
        /// Constructs a new instance ofthe <see cref="FileCommand"/> setting the db context.
        /// </summary>
        /// <param name="context">Instance of <see cref=""/></param>
        public FileCommand(IMvcForumContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to add a <see cref="File"/> to the database.
        /// </summary>
        /// <param name="file">The <see cref="File"/> to add.</param>
        /// <returns>The file Id.</returns>
        public Guid Create(File file)
        {
            file.UploadStatus = (int)Status.Uploading;
            var createdFile = _context.Files.Add(file);
            _context.SaveChanges();
            return createdFile.Id;
        }

        /// <summary>
        /// Method to update a <see cref="File"/>.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Guid Update(File file)
        {
            var dbFile = _context.Files.Find(file.Id);
            ((DbContext)_context).Entry(dbFile).State = EntityState.Modified;
            _context.SaveChanges();
            return file.Id;
        }
    }
}

using MvcForum.Core.Data.Context;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Database.Models;
using MvcForum.Core.Repositories.Models.FilesAndFolders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Command
{
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
            var createdFile = _context.Files.Add(file);
            _context.SaveChanges();
            return createdFile.Id;
        }

    }
}

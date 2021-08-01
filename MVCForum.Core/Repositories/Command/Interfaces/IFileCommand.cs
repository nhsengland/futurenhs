using MvcForum.Core.Repositories.Models.FilesAndFolders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcForum.Core.Models.Entities;

namespace MvcForum.Core.Repositories.Command.Interfaces
{
    /// <summary>
    /// Defines the interface for write interactions for files.
    /// </summary>
    public interface IFileCommand
    {
        /// <summary>
        /// Method to add a new <see cref="File"/> to the database.
        /// </summary>
        /// <param name="file">The <see cref="FileWriteViewModel"/> to add.</param>
        /// <returns>The id of the created file.</returns>
        Guid Create(File file);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MvcForum.Core.Models.FilesAndFolders;

namespace MvcForum.Core.Repositories.Command.Interfaces
{
    public interface IFolderCommand
    {
        Guid CreateFolder(FolderWriteViewModel folder);
        void UpdateFolder(FolderWriteViewModel folder);

        /// <summary>
        /// Delete a folder  by id - returns boolean to confirm deletion.
        /// </summary>
        /// <param name="folderId"><see cref="Guid"/> - Id of the folder.</param>
        /// <returns>Boolean -true if success</returns>
        Task<bool> DeleteFolderAsync(Guid folderId, CancellationToken cancellationToken = default(CancellationToken));
    }
}

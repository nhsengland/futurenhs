using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcForum.Core.Models.FilesAndFolders;

namespace MvcForum.Core.Repositories.Command.Interfaces
{
    public interface IFolderCommand
    {
        Guid CreateFolder(FolderWriteViewModel folder);
    }
}

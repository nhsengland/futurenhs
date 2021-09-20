using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Models.Entities;
using MvcForum.Core.Models.FilesAndFolders;
using MvcForum.Core.Repositories.Command.Interfaces;

namespace MvcForum.Core.Repositories.Command
{
    public class FolderCommand : IFolderCommand
    {
        private readonly IMvcForumContext _context;

        public FolderCommand(IMvcForumContext context)
        {
            _context = context;
        }

        public Guid CreateFolder(FolderWriteViewModel folder)
        {
            var newFolder = new Folder
            {
                Name = folder.FolderName,
                Description = folder.Description,
                AddedBy = folder.AddedBy,
                ParentFolder = folder.ParentFolder,
                DateAdded = DateTime.Now,
                ParentGroup = folder.ParentGroup
            };


            _context.Folder.Add(newFolder);
            _context.SaveChanges();
            return newFolder.Id;
        }

        public void UpdateFolder(FolderWriteViewModel folder)
        {
            var result = _context.Folder.FirstOrDefault(x => x.Id == folder.FolderId);

            if (result != null)
            {
                if (result.Name != folder.FolderName
                    || result.IsDeleted != folder.IsDeleted
                    || result.Description != folder.Description)
                {
                    result.Name = folder.FolderName;
                    result.Description = folder.Description;
                    result.IsDeleted = folder.IsDeleted;
                    _context.SaveChanges();
                }
            }
        }
    }
}

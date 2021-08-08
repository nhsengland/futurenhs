﻿using MvcForum.Core.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcForum.Core.Models.FilesAndFolders
{
    public class FileViewModel
    {
        public FileReadViewModel File { get; set; }

        public string Slug { get; set; }

        public IEnumerable<BreadCrumbItem> BreadCrumbTrail { get; set; }
    }
}

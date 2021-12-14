using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Models
{
    public class User
    {
        public Guid Id { get; }
        public string FullName { get; }
        public string Initials { get; }
        public Image UserAvatar { get; }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcForum.Core.Models.Enums
{
    public enum GroupUserStatus
    {
        NotLoggedIn,
        NotJoined,
        Pending,
        Joined,
        Locked,
        Banned,
        Rejected
    }
}

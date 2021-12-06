using MvcForum.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcForum.Core.Models.GroupAddMember
{
    public sealed class GroupAddMemberQueryResponse
    {        
        public GroupAddMemberQueryResponse(bool isApproved, ResponseType response)
        {
            if (!Enum.IsDefined(typeof(ResponseType), response)) throw new ArgumentOutOfRangeException(nameof(response), "Enum is not defined.");

            IsApproved = isApproved;
            Response = response;
        }
        public bool IsApproved { get; }
        public ResponseType Response { get; }        
    }
}

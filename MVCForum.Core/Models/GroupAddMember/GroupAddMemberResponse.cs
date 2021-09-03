using MvcForum.Core.Models.Enums;
using System;

namespace MvcForum.Core.Models.GroupAddMember
{
    public sealed class GroupAddMemberResponse
    {
        public GroupAddMemberResponse(Guid? groupUserId, ResponseType response)
        {
            if (Guid.Empty == groupUserId) throw new ArgumentOutOfRangeException(nameof(groupUserId), "Invalid use of empty guid.");
            if (!Enum.IsDefined(typeof(ResponseType), response)) throw new ArgumentOutOfRangeException(nameof(response), "Enum is not defined.");

            GroupUserId = groupUserId;
            Response = response;
        }
        public Guid? GroupUserId { get; }
        public ResponseType Response { get; }
    }
}

using System.Diagnostics.Contracts;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed class GroupUserDto
    {
        public Guid Id { get; set; }
        public bool Approved { get; set; }
        public bool Rejected { get; set; }
        public bool Locked { get; set; }
        public bool Banned { get; set; }
        public DateTime? RequestToJoinDateUTC { get; set; }
        public DateTime? ApprovedDateUTC { get; set; }
        public string RequestToJoinReason { get; set; }
        public string LockReason { get; set; }
        public string BanReason { get; set; }
        public Guid? ApprovingMembershipUser { get; set; }
        public Guid MembershipRole { get; set; }
        public Guid MembershipUser { get; set; }
        public Guid Group { get; set; }
        public byte[] RowVersion { get; set; }

        [ContractInvariantMethod]
        private void ClassInvariant()
        {
            Contract.Assert(MembershipRole != Guid.Empty);
            Contract.Assert(MembershipUser != Guid.Empty);
            Contract.Assert(Group != Guid.Empty);
            Contract.Assert(Approved && Rejected is false && Locked is false && Banned is false);
            Contract.Assert(Rejected && Approved is false && Locked is false && Banned is false);
            Contract.Assert(Locked && Approved is false && Rejected is false && Banned is false);
            Contract.Assert(Banned && Approved is false && Rejected is false && Locked is false);
        }
    }
}

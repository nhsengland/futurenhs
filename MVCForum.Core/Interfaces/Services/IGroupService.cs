namespace MvcForum.Core.Interfaces.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Models;
    using Models.Entities;
    using Models.General;
    using Pipeline;

    public partial interface IGroupService : IContextService
    {
        List<GroupUser> GetAllForUser(Guid? userId);

        List<Group> GetAll(Guid? membershipId);
        IEnumerable<Group> GetAllMainGroups(Guid? membershipId);
        IEnumerable<GroupSummary> GetAllMainGroupsInSummary(Guid? membershipId);
        ILookup<Guid, GroupSummary> GetAllMainGroupsInSummaryGroupedBySection(Guid? membershipId);

        IEnumerable<GroupSummary> GetAllMyGroupsInSummary(Guid? membershipId);

        /// <summary>
        ///     Gets Groups that the user has access to (i.e. There access is not denied)
        /// </summary>
        /// <param name="role">Users Role</param>
        /// <returns></returns>
        List<Group> GetAllowedGroups(MembershipRole role, Guid? membershipId);

        /// <summary>
        ///     Get Group permissions for a specific permission
        /// </summary>
        /// <param name="role">Users Role</param>
        /// <param name="actionType">
        ///     Pass in the permission you want to check, for example 'Delete Posts' - This will return a list
        ///     of Groups that the user has permission to delete posts
        /// </param>
        /// <returns></returns>
        List<Group> GetAllowedGroups(MembershipRole role, string actionType, Guid? membershipId);
        IEnumerable<Group> GetAllSubGroups(Guid parentId, Guid? membershipId);
        Group Get(Guid id);
        IList<Group> Get(IList<Guid> ids, bool fullGraph = false);
        GroupWithSubGroups GetBySlugWithSubGroups(string slug, Guid? membershipId);
        Group Get(string slug);
        List<Group> GetGroupParents(Group Group, List<Group> allowedGroups);
        Task<IPipelineProcess<Group>> Delete(Group Group);
        Task<IPipelineProcess<Group>> Create(Group Group, HttpPostedFileBase[] postedFiles, Guid? parentGroup, Guid? section);
        Task<IPipelineProcess<Group>> Edit(Group Group, HttpPostedFileBase[] postedFiles, Guid? parentGroup, Guid? section);
        void UpdateSlugFromName(Group Group, Guid MembershipId);
        Group SanitizeGroup(Group Group);
        List<Group> GetSubGroups(Group Group, List<Group> allGroups, Guid? membershipId, int level = 2);
        List<SelectListItem> GetBaseSelectListGroups(List<Group> allowedGroups, Guid? membershipId);
        Group GetBySlug(string slug);
        IList<Group> GetBySlugLike(string slug);
        IList<Group> GetAllDeepSubGroups(Group Group);
        void SortPath(Group Group, Group parentGroup);
        List<Section> GetAllSections();
        Section GetSection(Guid id);
        void DeleteSection(Guid id);

        Task JoinGroupAsync(string slug, Guid membershipId, CancellationToken cancellationToken);
        Task<bool> JoinGroupApproveAsync(Guid groupId, Guid membershipId, CancellationToken cancellationToken);

        bool AddGroupAdministrators(string slug, List<Guid> membershipIds, Guid approvingUserId);
        bool LeaveGroup(string slug, Guid membershipId);

        bool ApproveJoinGroup(Guid groupUserId, Guid approvingUserId);
        bool RejectJoinGroup(Guid groupUserId, Guid approvingUserId);

        MembershipRole GetGroupRole(Guid groupId, Guid? membershipId);

        GroupUser GetGroupUser(Guid groupUserId);
        Task<GroupUser> UpdateGroupUserAsync(GroupUser groupUser, CancellationToken cancellationToken);
    }
}
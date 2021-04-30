namespace MvcForum.Core.Interfaces.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Models;
    using Models.Entities;
    using Models.General;
    using Pipeline;

    public partial interface IGroupService : IContextService
    {
        List<Group> GetAll();
        IEnumerable<Group> GetAllMainGroups();
        IEnumerable<GroupSummary> GetAllMainGroupsInSummary();
        ILookup<Guid, GroupSummary> GetAllMainGroupsInSummaryGroupedBySection();

        /// <summary>
        ///     Gets Groups that the user has access to (i.e. There access is not denied)
        /// </summary>
        /// <param name="role">Users Role</param>
        /// <returns></returns>
        List<Group> GetAllowedGroups(MembershipRole role);

        /// <summary>
        ///     Get Group permissions for a specific permission
        /// </summary>
        /// <param name="role">Users Role</param>
        /// <param name="actionType">
        ///     Pass in the permission you want to check, for example 'Delete Posts' - This will return a list
        ///     of Groups that the user has permission to delete posts
        /// </param>
        /// <returns></returns>
        List<Group> GetAllowedGroups(MembershipRole role, string actionType);
        IEnumerable<Group> GetAllSubGroups(Guid parentId);
        Group Get(Guid id);
        IList<Group> Get(IList<Guid> ids, bool fullGraph = false);
        GroupWithSubGroups GetBySlugWithSubGroups(string slug);
        Group Get(string slug);
        List<Group> GetGroupParents(Group Group, List<Group> allowedGroups);
        Task<IPipelineProcess<Group>> Delete(Group Group);
        Task<IPipelineProcess<Group>> Create(Group Group, HttpPostedFileBase[] postedFiles, Guid? parentGroup, Guid? section);
        Task<IPipelineProcess<Group>> Edit(Group Group, HttpPostedFileBase[] postedFiles, Guid? parentGroup, Guid? section);
        void UpdateSlugFromName(Group Group);
        Group SanitizeGroup(Group Group);
        List<Group> GetSubGroups(Group Group, List<Group> allGroups, int level = 2);
        List<SelectListItem> GetBaseSelectListGroups(List<Group> allowedGroups);
        Group GetBySlug(string slug);
        IList<Group> GetBySlugLike(string slug);
        IList<Group> GetAllDeepSubGroups(Group Group);
        void SortPath(Group Group, Group parentGroup);
        List<Section> GetAllSections();
        Section GetSection(Guid id);
        void DeleteSection(Guid id);
    }
}
using MvcForum.Core.Models.Enums;

namespace MvcForum.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using Constants;
    using Interfaces;
    using Interfaces.Pipeline;
    using Interfaces.Services;
    using Microsoft.Owin.Infrastructure;
    using Models;
    using Models.Entities;
    using Models.General;
    using MvcForum.Core.ExtensionMethods;
    using MvcForum.Core.Models.Groups;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using Pipeline;
    using Reflection;
    using Utilities;

    public partial class GroupService : IGroupService
    {
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;
        private readonly IGroupPermissionForRoleService _groupPermissionForRoleService;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupCommand _groupCommand;
        private IMvcForumContext _context;
        private readonly IRoleService _roleService;
        private readonly ILocalizationService _localizationService;
        private readonly IImageService _imageService;
        private readonly IImageCommand _imageCommand;
        private readonly IImageRepository _imageRepository;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roleService"> </param>
        /// <param name="notificationService"> </param>
        /// <param name="GroupPermissionForRoleService"></param>
        /// <param name="cacheService"></param>
        public GroupService(IMvcForumContext context, IRoleService roleService,
                            INotificationService notificationService, IGroupPermissionForRoleService GroupPermissionForRoleService,
                            ICacheService cacheService, IGroupRepository groupRepository, ILocalizationService localizationService,
                            IGroupCommand groupCommand, IImageService imageService, IImageCommand imageCommand,
                            IImageRepository imageRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _groupPermissionForRoleService = GroupPermissionForRoleService ?? throw new ArgumentNullException(nameof(GroupPermissionForRoleService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(context));
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _groupCommand = groupCommand ?? throw new ArgumentNullException(nameof(groupCommand));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _imageCommand = imageCommand ?? throw new ArgumentNullException(nameof(imageCommand));
            _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        }

        /// <inheritdoc />
        public void RefreshContext(IMvcForumContext context)
        {
            _context = context;
            _roleService.RefreshContext(context);
            _notificationService.RefreshContext(context);
            _groupPermissionForRoleService.RefreshContext(context);
        }

        /// <inheritdoc />
        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        ///     Return all Groups
        /// </summary>
        /// <returns></returns>
        public List<Group> GetAll(Guid? membershipId)
        {
            var cachedGroups = _cacheService.Get<List<Group>>("GroupList.GetAll");
            if (cachedGroups == null)
            {
                var orderedGroups = new List<Group>();
                var allCats = _context.Group
                    .Include(x => x.ParentGroup)
                    .AsNoTracking()
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                foreach (var parentGroup in allCats.Where(x => x.ParentGroup == null).OrderBy(x => x.SortOrder))
                {
                    // Add the main Group
                    parentGroup.Level = 1;
                    orderedGroups.Add(parentGroup);

                    // Add subGroups under this
                    orderedGroups.AddRange(GetSubGroups(parentGroup, allCats, membershipId));
                }
                cachedGroups = orderedGroups;
            }
            return cachedGroups;
        }

        public List<GroupUser> GetAllForUser(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(userId));

            return _context.GroupUser.Where(x => x.User.Id == userId && x.Approved)
                .OrderBy(x => x.Group.Name).ToList();
        }

        public List<Group> DiscoverAllForUser(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(userId));

            return _context.Group
                .Where(x => !x.GroupUsers.Where(m => m.User.Id == userId).Any(y => y.Approved))
                .OrderBy(x => x.Name).ToList();
        }

        public List<Group> GetSubGroups(Group Group, List<Group> allGroups, Guid? membershipId, int level = 2)
        {

            var catsToReturn = new List<Group>();
            var cats = allGroups.Where(x => x.ParentGroup != null && x.ParentGroup.Id == Group.Id)
                .OrderBy(x => x.SortOrder);
            foreach (var cat in cats)
            {
                cat.Level = level;
                catsToReturn.Add(cat);
                catsToReturn.AddRange(GetSubGroups(cat, allGroups, membershipId, level + 1));
            }

            return catsToReturn;
        }

        public List<SelectListItem> GetBaseSelectListGroups(List<Group> allowedGroups, Guid? membershipId)
        {
            var cats = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            foreach (var cat in allowedGroups)
            {
                var catName = string.Concat(LevelDashes(cat.Level), cat.Level > 1 ? " " : "", cat.Name);
                cats.Add(new SelectListItem { Text = catName, Value = cat.Id.ToString() });
            }
            return cats;

        }

        /// <summary>
        ///     Return all sub Groups from a parent Group id
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public IEnumerable<Group> GetAllSubGroups(Guid parentId, Guid? membershipId)
        {
            return _context.Group.AsNoTracking()
                .Where(x => x.ParentGroup.Id == parentId)
                .OrderBy(x => x.SortOrder);
        }

        /// <summary>
        ///     Get all main Groups (Groups with no parent Group)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Group> GetAllMainGroups(Guid? membershipId)
        {
            return _context.Group.AsNoTracking()
                .Include(x => x.ParentGroup)
                .Where(cat => cat.ParentGroup == null)
                .OrderBy(x => x.SortOrder)
                .ToList();
        }

        /// <inheritdoc />
        public IEnumerable<GroupSummary> GetAllMainGroupsInSummary(Guid? membershipId)
        {
            return _context.Group.AsNoTracking()
                .Include(group => group.ParentGroup)
                .Include(group => group.Section)
                .Where(group => group.ParentGroup == null)
                .OrderBy(group => group.SortOrder)
                .Select(group => new GroupSummary
                {
                    Group = group,
                    DiscussionCount = group.Topics.Count,
                    MemberCount = group.GroupUsers.Count(g => g.Approved == true)
                })
                .ToList();
        }

        /// <inheritdoc />
        public ILookup<Guid, GroupSummary> GetAllMainGroupsInSummaryGroupedBySection(Guid? membershipId)
        {
            return _context.GroupUser.AsNoTracking()
                .Include(groupUser => groupUser.Group.ParentGroup)
                .Include(groupUser => groupUser.Group.Section)
                .Where(groupUser => groupUser.Group.ParentGroup == null && groupUser.Group.Section != null)
                .OrderBy(groupUser => groupUser.Group.SortOrder)
                .Select(groupUser => new GroupSummary
                {
                    Group = groupUser.Group,
                    DiscussionCount = groupUser.Group.Topics.Count,
                    MemberCount = groupUser.Group.GroupUsers.Count(g => g.Approved == true)
                })
                .ToList()
                .ToLookup(groupSummary => groupSummary.Group.Section.Id);
        }

        public IEnumerable<GroupSummary> GetAllMyGroupsInSummary(Guid membershipId)
        {
            if (membershipId == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(membershipId));

            return GetAllForUser(membershipId)
                .Select(groupUser => new GroupSummary
                {
                    Group = groupUser.Group,
                    DiscussionCount = groupUser.Group.Topics.Count,
                    MemberCount = groupUser.Group.GroupUsers.Count(g => g.Approved == true)
                })
                .ToList();
        }

        public IEnumerable<GroupSummary> GetDiscoverGroupsInSummary(Guid membershipId)
        {
            if (membershipId == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(membershipId));

            return DiscoverAllForUser(membershipId)
                .Select(group => new GroupSummary
                {
                    Group = group,
                    DiscussionCount = group.Topics.Count,
                    MemberCount = group.GroupUsers.Count(g => g.Approved == true)
                })
                .ToList();
        }

        /// <summary>
        ///     Return allowed Groups based on the users role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public List<Group> GetAllowedGroups(MembershipRole role, Guid? membershipId)
        {
            if (role.RoleName == Constants.AdminRoleName)
            {
                return GetAllowedGroups(role, ForumConfiguration.Instance.PermissionDenyAccess, membershipId);
            }

            return _context.GroupUser.Where(x => x.User.Id == membershipId && (x.Approved && !x.Banned && !x.Locked && !x.Rejected)).Select(x => x.Group).ToList();

        }



        public List<Group> GetAllowedGroups(MembershipRole role, string actionType, Guid? membershipId)
        {
            return GetAllowedGroupsCode(role, actionType, membershipId);
        }

        /// <summary>
        ///     Add a new Group
        /// </summary>
        /// <param name="Group"></param>
        /// <param name="postedFiles"></param>
        /// <param name="parentGroup"></param>
        /// <param name="section"></param>
        public async Task<IPipelineProcess<Group>> Create(Group Group, HttpPostedFileBase[] postedFiles, Guid? parentGroup, Guid? section)
        {
            // Get the pipelines
            var GroupPipes = ForumConfiguration.Instance.PipelinesGroupCreate;

            // The model to process
            var piplineModel = new PipelineProcess<Group>(Group);

            // Add parent Group
            if (parentGroup != null)
            {
                piplineModel.ExtendedData.Add(Constants.ExtendedDataKeys.ParentGroup, parentGroup);
            }

            // Add section
            if (section != null)
            {
                piplineModel.ExtendedData.Add(Constants.ExtendedDataKeys.Section, section);
            }

            // Add posted files
            if (postedFiles != null && postedFiles.Any(x => x != null))
            {
                piplineModel.ExtendedData.Add(Constants.ExtendedDataKeys.PostedFiles, postedFiles);
            }

            piplineModel.ExtendedData.Add(Constants.ExtendedDataKeys.Username, HttpContext.Current.User.Identity.Name);

            // Get instance of the pipeline to use
            var pipeline = new Pipeline<IPipelineProcess<Group>, Group>(_context);

            // Register the pipes 
            var allGroupPipes = ImplementationManager.GetInstances<IPipe<IPipelineProcess<Group>>>();

            // Loop through the pipes and add the ones we want
            foreach (var pipe in GroupPipes)
            {
                if (allGroupPipes.ContainsKey(pipe))
                {
                    pipeline.Register(allGroupPipes[pipe]);
                }
            }

            return await pipeline.Process(piplineModel);
        }

        /// <inheritdoc />
        public async Task<IPipelineProcess<Group>> Edit(Group Group, HttpPostedFileBase[] postedFiles, Guid? parentGroup, Guid? section)
        {
            // Get the pipelines
            var GroupPipes = ForumConfiguration.Instance.PipelinesGroupUpdate;

            // The model to process
            var piplineModel = new PipelineProcess<Group>(Group);

            // Add parent Group
            if (parentGroup != null)
            {
                piplineModel.ExtendedData.Add(Constants.ExtendedDataKeys.ParentGroup, parentGroup);
            }

            // Add section
            if (section != null)
            {
                piplineModel.ExtendedData.Add(Constants.ExtendedDataKeys.Section, section);
            }

            // Add posted files
            if (postedFiles != null && postedFiles.Any(x => x != null))
            {
                piplineModel.ExtendedData.Add(Constants.ExtendedDataKeys.PostedFiles, postedFiles);
            }

            piplineModel.ExtendedData.Add(Constants.ExtendedDataKeys.Username, HttpContext.Current.User.Identity.Name);
            piplineModel.ExtendedData.Add(Constants.ExtendedDataKeys.IsEdit, true);

            // Get instance of the pipeline to use
            var pipeline = new Pipeline<IPipelineProcess<Group>, Group>(_context);

            // Register the pipes 
            var allGroupPipes = ImplementationManager.GetInstances<IPipe<IPipelineProcess<Group>>>();

            // Loop through the pipes and add the ones we want
            foreach (var pipe in GroupPipes)
            {
                if (allGroupPipes.ContainsKey(pipe))
                {
                    pipeline.Register(allGroupPipes[pipe]);
                }
            }

            return await pipeline.Process(piplineModel);
        }

        /// <summary>
        ///     Keep slug in line with name
        /// </summary>
        /// <param name="Group"></param>
        public void UpdateSlugFromName(Group Group, Guid MembershipId)
        {
            // Sanitize
            Group = SanitizeGroup(Group);

            var updateSlug = true;

            // Check if slug has changed as this could be an update
            if (!string.IsNullOrWhiteSpace(Group.Slug))
            {
                var GroupBySlug = GetBySlugWithSubGroups(Group.Slug, MembershipId);
                if (GroupBySlug.Group.Id == Group.Id)
                {
                    updateSlug = false;
                }
            }

            if (updateSlug)
            {
                Group.Slug = ServiceHelpers.GenerateSlug(Group.Name, GetBySlugLike(Group.Slug).Select(x => x.Slug).ToList(), Group.Slug);
            }
        }

        /// <summary>
        ///     Sanitizes a Group
        /// </summary>
        /// <param name="Group"></param>
        /// <returns></returns>
        public Group SanitizeGroup(Group Group)
        {
            // Sanitize any strings in a Group
            Group.Description = StringUtils.GetSafeHtml(Group.Description);
            Group.Name = HttpUtility.HtmlDecode(StringUtils.SafePlainText(Group.Name));
            return Group;
        }

        /// <summary>
        ///     Return Group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Group Get(Guid id)
        {
            return _context.Group.FirstOrDefault(x => x.Id == id);
        }

        public IList<Group> Get(IList<Guid> ids, bool fullGraph = false)
        {
            IList<Group> Groups;

            if (fullGraph)
            {
                Groups =
                    _context.Group.AsNoTracking()
                        .Include(x => x.Topics.Select(l => l.LastPost.User))
                        .Include(x => x.ParentGroup)
                        .Where(x => ids.Contains(x.Id))
                        .ToList();
            }
            else
            {
                Groups = _context.Group
                    .AsNoTracking().Where(x => ids.Contains(x.Id)).ToList();
            }

            // make sure Groups are returned in order of ids (not in Database order)
            return ids.Select(id => Groups.Single(c => c.Id == id)).ToList();
        }

        /// <summary>
        ///     Return model with Sub Groups
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public GroupWithSubGroups GetBySlugWithSubGroups(string slug, Guid? membershipId)
        {
            slug = StringUtils.SafePlainText(slug);
            var group = (from Group in _context.Group.AsNoTracking()
                         where Group.Slug == slug
                         select new GroupWithSubGroups
                         {
                             Group = Group,
                             SubGroups = from cats in _context.Group
                                         where cats.ParentGroup.Id == Group.Id
                                         select cats
                         }).FirstOrDefault();

            return group;
        }

        /// <summary>
        ///     Return Group by Url slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<GroupViewModel> GetAsync(string slug, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));

            return await GetBySlugAsync(slug, cancellationToken);
        }

        public GroupViewModel Get(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));

            return GetBySlug(slug);

        }

        /// <summary>
        ///     Return Group by Url slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<GroupViewModel> GetAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Guid.Empty == id) throw new ArgumentOutOfRangeException(nameof(id));

            return await _groupRepository.GetGroupAsync(id, cancellationToken);
        }

        /// <summary>
        ///     Gets the Group parents
        /// </summary>
        /// <param name="Group"></param>
        /// <param name="allowedGroups"></param>
        /// <returns></returns>
        public List<Group> GetGroupParents(Group Group, List<Group> allowedGroups)
        {
            var path = Group.Path;
            var cats = new List<Group>();
            if (!string.IsNullOrWhiteSpace(path))
            {
                var catGuids = path.Trim().Split(',').Select(x => new Guid(x)).ToList();
                if (!catGuids.Contains(Group.Id))
                {
                    catGuids.Add(Group.Id);
                }
                cats = Get(catGuids).ToList();
            }
            var allowedCatIds = new List<Guid>();
            if (allowedGroups.Any())
            {
                allowedCatIds.AddRange(allowedGroups.Select(x => x.Id));
            }
            return cats.Where(x => allowedCatIds.Contains(x.Id)).ToList();
        }

        /// <summary>
        ///     Delete a Group
        /// </summary>
        /// <param name="Group"></param>
        public async Task<IPipelineProcess<Group>> Delete(Group Group)
        {
            // Get the pipelines
            var GroupPipes = ForumConfiguration.Instance.PipelinesGroupDelete;

            // The model to process
            var piplineModel = new PipelineProcess<Group>(Group);

            // Get instance of the pipeline to use
            var pipeline = new Pipeline<IPipelineProcess<Group>, Group>(_context);

            // Register the pipes 
            var allGroupPipes = ImplementationManager.GetInstances<IPipe<IPipelineProcess<Group>>>();

            // Loop through the pipes and add the ones we want
            foreach (var pipe in GroupPipes)
            {
                if (allGroupPipes.ContainsKey(pipe))
                {
                    pipeline.Register(allGroupPipes[pipe]);
                }
            }

            return await pipeline.Process(piplineModel);
        }

        public async Task<GroupViewModel> GetBySlugAsync(string slug, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));

            slug = StringUtils.GetSafeHtml(slug);
            return await _groupRepository.GetGroupAsync(slug, cancellationToken);
        }

        public GroupViewModel GetBySlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));

            slug = StringUtils.GetSafeHtml(slug);
            return _groupRepository.GetGroup(slug);
        }

        public IList<Group> GetBySlugLike(string slug)
        {
            slug = StringUtils.GetSafeHtml(slug);

            return _context.Group.AsNoTracking()
                .Where(x => x.Slug.Contains(slug))
                .ToList();
        }

        /// <summary>
        ///     Gets all Groups right the way down
        /// </summary>
        /// <param name="Group"></param>
        /// <returns></returns>
        public IList<Group> GetAllDeepSubGroups(Group Group)
        {
            var catGuid = Group.Id.ToString().ToLower();
            return _context.Group
                .Where(x => x.Path != null && x.Path.ToLower().Contains(catGuid))
                .OrderBy(x => x.SortOrder)
                .ToList();
        }

        /// <inheritdoc />
        public void SortPath(Group Group, Group parentGroup)
        {
            // Append the path from the parent Group
            var path = !string.IsNullOrWhiteSpace(parentGroup.Path) ?
                string.Concat(parentGroup.Path, ",", parentGroup.Id.ToString()) :
                parentGroup.Id.ToString();

            Group.Path = path;
        }

        /// <inheritdoc />
        public List<Section> GetAllSections()
        {
            return _context.Section.AsNoTracking().Include(x => x.Groups).OrderBy(x => x.SortOrder).ToList();
        }

        /// <inheritdoc />
        public Section GetSection(Guid id)
        {
            return _context.Section.Find(id);
        }

        /// <inheritdoc />
        public void DeleteSection(Guid id)
        {
            var section = _context.Section.Find(id);
            if (section != null)
            {
                _context.Section.Remove(section);
                _context.SaveChanges();
            }
        }

        private static string LevelDashes(int level)
        {
            if (level > 1)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < level - 1; i++)
                {
                    sb.Append("-");
                }
                return sb.ToString();
            }
            return string.Empty;
        }

        private List<Group> GetAllowedGroupsCode(MembershipRole role, string actionType, Guid? membershipId)
        {
            var filteredCats = new List<Group>();
            var allCats = GetAll(membershipId);
            foreach (var Group in allCats)
            {
                var permissionSet = _roleService.GetPermissions(Group, role);
                if (!permissionSet[actionType].IsTicked)
                {
                    // Only add it Group is NOT locked
                    filteredCats.Add(Group);
                }
            }
            return filteredCats;
        }


        public async Task JoinGroupAsync(string slug, Guid membershipId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new ArgumentNullException(nameof(slug));
            }
            
            if (Guid.Empty == membershipId)
            {
                throw new ArgumentOutOfRangeException(nameof(membershipId));
            }

            var group = await _context.Group.SingleAsync(x => x.Slug == slug.ToLower(), cancellationToken);


            group.GroupUsers = new List<GroupUser>();
            var dateTimeUtcNow = DateTime.UtcNow;

            var groupUser = new GroupUser()
            {
                Approved = group.PublicGroup,
                Rejected = false,
                Banned = false,
                Locked = false,
                Group = group,
                RequestToJoinDate = dateTimeUtcNow,
                Role = await _context.MembershipRole.SingleAsync(x => x.RoleName == Constants.GuestRoleName, cancellationToken),
                User = await _context.MembershipUser.SingleAsync(x => x.Id == membershipId, cancellationToken)
            };

            if (groupUser.Approved)
            {
                groupUser.ApprovedToJoinDate = dateTimeUtcNow;
            }

            _context.GroupUser.Add(groupUser);
            int resultCount = await _context.SaveChangesAsync(cancellationToken);

            if (resultCount == 0)
            {
                throw new DbUpdateException(nameof(groupUser)); 
            }            
        }

        public async Task<bool> JoinGroupApproveAsync(Guid groupId, Guid membershipId, CancellationToken cancellationToken)
        {
            if (Guid.Empty == groupId)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }

            if (Guid.Empty == membershipId)
            {
                throw new ArgumentOutOfRangeException(nameof(membershipId));
            }

            var group = await _context.Group.SingleAsync(x => x.Id == groupId, cancellationToken);
            group.GroupUsers = new List<GroupUser>();
            var dateTimeUtcNow = DateTime.UtcNow;

            var groupUser = new GroupUser()
            {
                Approved = true,
                Rejected = false,
                Banned = false,
                Locked = false,
                Group = group,
                RequestToJoinDate = dateTimeUtcNow,
                ApprovedToJoinDate = dateTimeUtcNow,
                Role = await _context.MembershipRole.SingleAsync(x => x.RoleName == Constants.StandardRoleName, cancellationToken),
                User = await _context.MembershipUser.SingleAsync(x => x.Id == membershipId, cancellationToken)
            };

            _context.GroupUser.Add(groupUser);
            int resultCount = await _context.SaveChangesAsync(cancellationToken);

            if (resultCount == 0)
            {
                return false;
            }

            return true;
        }

        public bool AddGroupAdministrators(string slug, List<Guid> membershipIds, Guid approvingUserId)
        {

            var group = _context.Group.SingleOrDefault(x => x.Slug == slug);

            foreach (var membershipId in membershipIds)
            {
                var dateTimeUtcNow = DateTime.UtcNow;
                var groupUser = new GroupUser();
                groupUser.Approved = true;
                groupUser.Rejected = false;
                groupUser.Banned = false;
                groupUser.Locked = false;
                groupUser.Group = group;
                groupUser.RequestToJoinDate = dateTimeUtcNow;
                groupUser.Role = _context.MembershipRole.FirstOrDefault(x => x.RoleName == Constants.AdminRoleName);
                groupUser.User = _context.MembershipUser.FirstOrDefault(x => x.Id == membershipId);
                groupUser.ApprovedToJoinDate = dateTimeUtcNow;
                groupUser.ApprovingUser = _context.MembershipUser.FirstOrDefault(x => x.Id == approvingUserId);                

                if (@group != null && group.GroupUsers == null || !@group.GroupUsers.Any(x =>
                    x.User.Id == membershipId && x.Group.Id == group.Id))
                {
                    _context.GroupUser.Add(groupUser);
                }

                if (group.GroupUsers == null || group.GroupUsers.Any(x =>
                    x.User.Id == membershipId && x.Role.RoleName != Constants.AdminRoleName))
                {
                    var user = _context.GroupUser.FirstOrDefault(x => x.User.Id == membershipId && x.Group.Id == group.Id);
                    if (user != null)
                    {
                        user.Approved = true;
                        user.Rejected = false;
                        user.Banned = false;
                        user.Locked = false;
                        user.Role = _context.MembershipRole.FirstOrDefault(x => x.RoleName == Constants.AdminRoleName);
                    }
                }
                _ = _context.SaveChanges();
            }


            if (group.GroupUsers != null)
            {
                var removedUsers = group.GroupUsers.Where(x =>
                    x.Role.RoleName == Constants.AdminRoleName && x.Group.Id == group.Id && !membershipIds.Contains(x.User.Id));

                _context.GroupUser.RemoveRange(removedUsers);

                _ = _context.SaveChanges();
            }

            return true;
        }

        public bool ApproveJoinGroup(Guid groupUserId, Guid approvingUserId)
        {
            var groupUser = _context.GroupUser.SingleOrDefault(x => x.Id == groupUserId);
            if (groupUser != null)
            {
                groupUser.Approved = true;
                groupUser.Rejected = false;
                groupUser.ApprovedToJoinDate = DateTime.UtcNow;
                groupUser.Role = _context.MembershipRole.FirstOrDefault(x => x.RoleName == Constants.StandardRoleName);
                groupUser.ApprovingUser = _context.MembershipUser.FirstOrDefault(x => x.Id == approvingUserId);
            }

            _ = _context.SaveChanges();
            return true;
        }

        public bool RejectJoinGroup(Guid groupUserId, Guid approvingUserId)
        {
            var groupUser = _context.GroupUser.SingleOrDefault(x => x.Id == groupUserId);
            if (groupUser != null)
            {
                groupUser.Rejected = true;
                groupUser.ApprovedToJoinDate = DateTime.UtcNow;
                groupUser.ApprovingUser = _context.MembershipUser.FirstOrDefault(x => x.Id == approvingUserId);
            }

            _ = _context.SaveChanges();
            return true;
        }

        public async Task<bool> LeaveGroupAsync(string slug, Guid membershipId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));
            if (Guid.Empty == membershipId) throw new ArgumentOutOfRangeException(nameof(membershipId));

            var group = await GetBySlugAsync(slug);
            var groupUser = _context.GroupUser.FirstOrDefault(x => x.Group.Id == group.Id && x.User.Id == membershipId);
            if (groupUser != null)
            {
                _context.GroupUser.Remove(groupUser);
                _ = _context.SaveChanges();
                return true;
            }

            return false;
        }

        public MembershipRole GetGroupRole(Guid groupId, Guid? membershipId)
        {
            if (membershipId == null)
                return _context.MembershipRole.AsNoTracking().FirstOrDefault(x => x.RoleName == Constants.GuestRoleName);

            var groupUser = _context.GroupUser.AsNoTracking().FirstOrDefault(x => x.Group.Id == groupId && x.User.Id == membershipId);

            if (groupUser == null)
                return _context.MembershipRole.AsNoTracking().FirstOrDefault(x => x.RoleName == Constants.GuestRoleName);


            if (groupUser.GetUserStatusForGroup() != GroupUserStatus.Joined)
                return _context.MembershipRole.AsNoTracking().FirstOrDefault(x => x.RoleName == Constants.GuestRoleName);

            return groupUser?.Role;
        }

        public GroupUser GetGroupUser(Guid groupUserId)
        {
            var groupUser = _context.GroupUser.Include(x => x.Role).FirstOrDefault(x => x.Id == groupUserId);
            return groupUser;
        }

        public async Task<GroupUser> UpdateGroupUserAsync(GroupUser groupUser, CancellationToken cancellationToken)
        {
            var user = _context.GroupUser.Include(x => x.Role).FirstOrDefault(x => x.Id == groupUser.Id);
            if (user is null)
                return null;
            user.Approved = groupUser.Approved;
            user.Locked = groupUser.Locked;
            user.Banned = groupUser.Banned;
            user.Role = _context.MembershipRole.FirstOrDefault(x => x.Id == groupUser.Role.Id);
            _ = await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public bool UserIsAdmin(string groupSlug, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(groupSlug)) throw new ArgumentNullException(nameof(groupSlug));
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            return _groupRepository.UserIsAdmin(groupSlug, userId);
        }

        public bool UserHasGroupAccess(string groupSlug, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(groupSlug)) throw new ArgumentNullException(nameof(groupSlug));
            if (Guid.Empty == userId) throw new ArgumentOutOfRangeException(nameof(userId));

            return _groupRepository.UserHasGroupAccess(groupSlug, userId);
        }

        public async Task<bool> UpdateAsync(GroupWriteViewModel model, string slug, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));

            if (model is null) throw new ArgumentNullException(nameof(model));

            return await _groupCommand.UpdateAsync(model, slug, cancellationToken);
        }

        public UploadFileResult UploadGroupImage(HttpPostedFileBase file, Guid groupId)
        {
            if (file is null) throw new ArgumentNullException(nameof(file));
            if (Guid.Empty == groupId) throw new ArgumentOutOfRangeException(nameof(groupId));

            // Before we save anything, check the user already has an upload folder and if not create one
            var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(ForumConfiguration.Instance.UploadFolderPath, groupId));
            if (!Directory.Exists(uploadFolderPath))
            {
                Directory.CreateDirectory(uploadFolderPath ?? throw new InvalidOperationException());
            }

            return file.UploadFile(uploadFolderPath, _localizationService, _imageCommand, _imageRepository, _imageService, false, groupId);
        }
    }
}

namespace MvcForum.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Constants;
    using Interfaces;
    using Interfaces.Pipeline;
    using Interfaces.Services;
    using Models;
    using Models.Entities;
    using Models.General;
    using Pipeline;
    using Reflection;
    using Utilities;

    public partial class GroupService : IGroupService
    {
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;
        private readonly IGroupPermissionForRoleService _GroupPermissionForRoleService;
        private IMvcForumContext _context;
        private readonly IRoleService _roleService;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roleService"> </param>
        /// <param name="notificationService"> </param>
        /// <param name="GroupPermissionForRoleService"></param>
        /// <param name="cacheService"></param>
        public GroupService(IMvcForumContext context, IRoleService roleService,
            INotificationService notificationService,
            IGroupPermissionForRoleService GroupPermissionForRoleService, ICacheService cacheService)
        {
            _roleService = roleService;
            _notificationService = notificationService;
            _GroupPermissionForRoleService = GroupPermissionForRoleService;
            _cacheService = cacheService;
            _context = context;
        }

        /// <inheritdoc />
        public void RefreshContext(IMvcForumContext context)
        {
            _context = context;
            _roleService.RefreshContext(context);
            _notificationService.RefreshContext(context);
            _GroupPermissionForRoleService.RefreshContext(context);
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
        public List<Group> GetAll()
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
                    orderedGroups.AddRange(GetSubGroups(parentGroup, allCats));
                }
                cachedGroups = orderedGroups;
            }
            return cachedGroups;
        }

        public List<Group> GetSubGroups(Group Group, List<Group> allGroups, int level = 2)
        {

                var catsToReturn = new List<Group>();
                var cats = allGroups.Where(x => x.ParentGroup != null && x.ParentGroup.Id == Group.Id)
                    .OrderBy(x => x.SortOrder);
                foreach (var cat in cats)
                {
                    cat.Level = level;
                    catsToReturn.Add(cat);
                    catsToReturn.AddRange(GetSubGroups(cat, allGroups, level + 1));
                }

                return catsToReturn;
    
        }

        public List<SelectListItem> GetBaseSelectListGroups(List<Group> allowedGroups)
        {
                var cats = new List<SelectListItem> {new SelectListItem {Text = "", Value = ""}};
                foreach (var cat in allowedGroups)
                {
                    var catName = string.Concat(LevelDashes(cat.Level), cat.Level > 1 ? " " : "", cat.Name);
                    cats.Add(new SelectListItem {Text = catName, Value = cat.Id.ToString()});
                }
                return cats;
        
        }

        /// <summary>
        ///     Return all sub Groups from a parent Group id
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public IEnumerable<Group> GetAllSubGroups(Guid parentId)
        {
            return _context.Group.AsNoTracking()
                .Where(x => x.ParentGroup.Id == parentId)
                .OrderBy(x => x.SortOrder);
        }

        /// <summary>
        ///     Get all main Groups (Groups with no parent Group)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Group> GetAllMainGroups()
        {
            return _context.Group.AsNoTracking()
                .Include(x => x.ParentGroup)
                .Where(cat => cat.ParentGroup == null)
                .OrderBy(x => x.SortOrder)
                .ToList();
        }

        /// <inheritdoc />
        public IEnumerable<GroupSummary> GetAllMainGroupsInSummary()
        {
            return _context.Group.AsNoTracking()
                .Include(x => x.ParentGroup)
                .Include(x => x.Section)
                .Where(cat => cat.ParentGroup == null)
                .OrderBy(x => x.SortOrder)
                .Select(x => new GroupSummary
                {
                    Group = x,
                    TopicCount = x.Topics.Count,
                    PostCount = x.Topics.SelectMany(p => p.Posts).Count(), // TODO - Should this be a seperate call?
                    MostRecentTopic = x.Topics.OrderByDescending(t => t.LastPost.DateCreated).FirstOrDefault() // TODO - Should this be a seperate call?
                })
                .ToList();
        }

        /// <inheritdoc />
        public ILookup<Guid, GroupSummary> GetAllMainGroupsInSummaryGroupedBySection()
        {
            return _context.Group.AsNoTracking()
                .Include(x => x.ParentGroup)
                .Include(x => x.Section)
                .Where(x => x.ParentGroup == null && x.Section != null)
                .OrderBy(x => x.SortOrder)
                .Select(x => new GroupSummary
                {
                    Group = x,
                    TopicCount = x.Topics.Count,
                    PostCount = x.Topics.SelectMany(p => p.Posts).Count(), // TODO - Should this be a seperate call?
                    MostRecentTopic = x.Topics.OrderByDescending(t => t.LastPost.DateCreated).FirstOrDefault() // TODO - Should this be a seperate call?
                })
                .ToList()
                .ToLookup(x => x.Group.Section.Id);
        }

        /// <summary>
        ///     Return allowed Groups based on the users role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public List<Group> GetAllowedGroups(MembershipRole role)
        {
            return GetAllowedGroups(role, ForumConfiguration.Instance.PermissionDenyAccess);
        }

        public List<Group> GetAllowedGroups(MembershipRole role, string actionType)
        {
            return GetAllowedGroupsCode(role, actionType);
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
        public void UpdateSlugFromName(Group Group)
        {
            // Sanitize
            Group = SanitizeGroup(Group);

            var updateSlug = true;

            // Check if slug has changed as this could be an update
            if (!string.IsNullOrWhiteSpace(Group.Slug))
            {
                var GroupBySlug = GetBySlugWithSubGroups(Group.Slug);
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
        public GroupWithSubGroups GetBySlugWithSubGroups(string slug)
        {
            slug = StringUtils.SafePlainText(slug);

            var cat = (from Group in _context.Group.AsNoTracking()
                where Group.Slug == slug
                select new GroupWithSubGroups
                {
                    Group = Group,
                    SubGroups = from cats in _context.Group
                        where cats.ParentGroup.Id == Group.Id
                        select cats
                }).FirstOrDefault();

            return cat;
        }

        /// <summary>
        ///     Return Group by Url slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public Group Get(string slug)
        {
            return GetBySlug(slug);
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

        public Group GetBySlug(string slug)
        {
            slug = StringUtils.GetSafeHtml(slug);
            return _context.Group.AsNoTracking().FirstOrDefault(x => x.Slug == slug);
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

        private List<Group> GetAllowedGroupsCode(MembershipRole role, string actionType)
        {
            var filteredCats = new List<Group>();
            var allCats = GetAll();
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
    }
}
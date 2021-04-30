namespace MvcForum.Plugins.Pipelines.Group
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.ExtensionMethods;
    using Core.Interfaces;
    using Core.Interfaces.Pipeline;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using Core.Services;

    public class GroupCreateEditPipe : IPipe<IPipelineProcess<Group>>
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILoggingService _loggingService;
        private readonly IGroupService _GroupService;
        private readonly ICacheService _cacheService;

        public GroupCreateEditPipe(ILocalizationService localizationService, ILoggingService loggingService, IGroupService GroupService, ICacheService cacheService)
        {
            _localizationService = localizationService;
            _loggingService = loggingService;
            _GroupService = GroupService;
            _cacheService = cacheService;
        }

        /// <inheritdoc />
        public async Task<IPipelineProcess<Group>> Process(IPipelineProcess<Group> input,
            IMvcForumContext context)
        {
            _localizationService.RefreshContext(context);
            _GroupService.RefreshContext(context);

            try
            {
                Guid? parentGroupGuid = null;
                if (input.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.ParentGroup))
                {
                    parentGroupGuid = input.ExtendedData[Constants.ExtendedDataKeys.ParentGroup] as Guid?;
                }

                // Sort if this is a section
                Section section = null;
                if (input.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.Section))
                {
                    if (input.ExtendedData[Constants.ExtendedDataKeys.Section] is Guid guid)
                    {
                        section = _GroupService.GetSection(guid);
                    }
                }

                // Sort the section - If it's null remove it
                input.EntityToProcess.Section = section ?? null;

                var isEdit = false;
                if (input.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.IsEdit))
                {
                    isEdit = input.ExtendedData[Constants.ExtendedDataKeys.IsEdit] as bool? == true;
                }

                if (isEdit)
                {

                    var parentCat = parentGroupGuid != null
                        ? _GroupService.Get(parentGroupGuid.Value)
                        : null;

                    // Check they are not trying to add a subGroup of this Group as the parent or it will break
                    if (parentCat?.Path != null && parentGroupGuid != null)
                    {
                        var parentCats = parentCat.Path.Split(',').Where(x => !string.IsNullOrWhiteSpace(x))
                            .Select(x => new Guid(x)).ToList();
                        if (parentCats.Contains(input.EntityToProcess.Id))
                        {
                            // Remove the parent Group, but still let them create the catgory
                            parentGroupGuid = null;
                        }
                    }

                    if (parentGroupGuid != null)
                    {
                        // Set the parent Group
                        var parentGroup = _GroupService.Get(parentGroupGuid.Value);
                        input.EntityToProcess.ParentGroup = parentGroup;

                        // Append the path from the parent Group
                        _GroupService.SortPath(input.EntityToProcess, parentGroup);
                    }
                    else
                    {
                        // Must access property (trigger lazy-loading) before we can set it to null (Entity Framework bug!!!)
                        var triggerEfLoad = input.EntityToProcess.ParentGroup;
                        input.EntityToProcess.ParentGroup = null;

                        // Also clear the path
                        input.EntityToProcess.Path = null;
                    }

                    _GroupService.UpdateSlugFromName(input.EntityToProcess);

                    await context.SaveChangesAsync();
                }
                else
                {
                    if (parentGroupGuid != null)
                    {
                        var parentGroup = await context.Group.FirstOrDefaultAsync(x => x.Id == parentGroupGuid.Value);
                        input.EntityToProcess.ParentGroup = parentGroup;
                        _GroupService.SortPath(input.EntityToProcess, parentGroup);
                    }


                    // url slug generator
                    input.EntityToProcess.Slug = ServiceHelpers.GenerateSlug(input.EntityToProcess.Name,
                        _GroupService.GetBySlugLike(ServiceHelpers.CreateUrl(input.EntityToProcess.Name)).Select(x => x.Slug).ToList(), null);

                    // Add the Group
                    context.Group.Add(input.EntityToProcess);
                }

                await context.SaveChangesAsync();

                _cacheService.ClearStartsWith("GroupList");

            }
            catch (Exception ex)
            {
                input.AddError(ex.Message);
                _loggingService.Error(ex);
            }

            return input;
        }
    }
}


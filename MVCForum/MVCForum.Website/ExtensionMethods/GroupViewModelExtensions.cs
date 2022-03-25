namespace MvcForum.Web.ExtensionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models.Entities;
    using MvcForum.Core.Constants;
    using ViewModels.Admin;

    public static class GroupViewModelExtensions
    {
        /// <summary>
        /// Turns a Edit view model into a Group
        /// </summary>
        /// <param name="GroupViewModel"></param>
        /// <returns></returns>
        public static Group ToGroup(this GroupEditViewModel GroupViewModel)
        {
            var Group = new Group {
                Name = GroupViewModel.Name,
                Description = GroupViewModel.Description,
                Subtitle = GroupViewModel.Subtitle,
                Introduction = GroupViewModel.Introduction,
                IsLocked = GroupViewModel.IsLocked,
                ModeratePosts = GroupViewModel.ModeratePosts,
                ModerateTopics = GroupViewModel.ModerateTopics,
                SortOrder = GroupViewModel.SortOrder,
                PageTitle = GroupViewModel.PageTitle,
                MetaDescription = GroupViewModel.MetaDesc,
                Colour = GroupViewModel.GroupColour,
                CreatedAtUtc = DateTime.UtcNow,
                PublicGroup = GroupViewModel.Public
        };

            return Group;
        }

        /// <summary>
        /// Maps changed data to a Group
        /// </summary>
        /// <param name="GroupViewModel"></param>
        /// <param name="Group"></param>
        /// <returns></returns>
        public static Group ToGroup(this GroupEditViewModel GroupViewModel, Group Group)
        {
            Group.Description = GroupViewModel.Description;
            Group.Subtitle = GroupViewModel.Subtitle;
            Group.Introduction = GroupViewModel.Introduction;
            Group.IsLocked = GroupViewModel.IsLocked;
            Group.ModeratePosts = GroupViewModel.ModeratePosts;
            Group.ModerateTopics = GroupViewModel.ModerateTopics;
            Group.Name = GroupViewModel.Name;
            Group.SortOrder = GroupViewModel.SortOrder;
            Group.PageTitle = GroupViewModel.PageTitle;
            Group.MetaDescription = GroupViewModel.MetaDesc;
            Group.Colour = GroupViewModel.GroupColour;
            Group.PublicGroup = GroupViewModel.Public;

            return Group;
        }

        /// <summary>
        /// Maps a Group to the edit view model
        /// </summary>
        /// <param name="Group"></param>
        /// <param name="allGroupSelectListItems"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static GroupEditViewModel ToEditViewModel(this Group Group, List<SelectListItem> allGroupSelectListItems, IEnumerable<SelectListItem> sections)
        {
            var GroupViewModel = new GroupEditViewModel
            {
                Name = Group.Name,
                Description = Group.Description,
                Subtitle = Group.Subtitle,
                Introduction = Group.Introduction,
                IsLocked = Group.IsLocked,
                ModeratePosts = Group.ModeratePosts == true,
                ModerateTopics = Group.ModerateTopics == true,
                SortOrder = Group.SortOrder,
                Id = Group.Id,
                PageTitle = Group.PageTitle,
                MetaDesc = Group.MetaDescription,
                Image = Group.Image,
                GroupColour = Group.Colour,
                ParentGroup = Group.ParentGroup?.Id ?? Guid.Empty,
                Section = Group.Section?.Id ?? Guid.Empty,
                AllGroups = allGroupSelectListItems,
                AllSections = sections,
                GroupOwner = Group.GroupOwner?.Id,
                GroupAdministrators = Group.GroupUsers.Where(x => x.Role.RoleName == Constants.AdminRoleName).Select(x => x.User.Id),
                Public = Group.PublicGroup
            };
            return GroupViewModel;
        }

    }
}
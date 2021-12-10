namespace MvcForum.Web.ViewModels.Admin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;
    using Core.Constants;
    using Core.Models.Entities;

    public class ListGroupsViewModel
    {
        public IEnumerable<Group> Groups { get; set; }
    }

    public class GroupEditViewModel
    {
        public GroupEditViewModel()
        {
            AllGroups = new List<SelectListItem>();
            AllSections = new List<SelectListItem>();
        }

        [HiddenInput]
        public Guid Id { get; set; }

        [DisplayName("Group Name")]
        [Required]
        [StringLength(600)]
        public string Name { get; set; }

        [DisplayName("Strap line")]
        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(1800)]
        public string Description { get; set; }

        [DisplayName("Group subtitle")]
        [StringLength(254, ErrorMessage = "The group subtitle must be less than 255 characters.")]
        public string Subtitle { get; set; }

        [DisplayName("Group introduction")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Please provide an introduction to your group")]
        [StringLength(4000, ErrorMessage = "The group introduction must not be greater than 4000 characters.")]
        public string Introduction { get; set; }

        [DisplayName("Group Colour")]
        [UIHint(Constants.EditorTemplateColourPicker)]
        [AllowHtml]
        public string GroupColour { get; set; }

        [DisplayName("Lock The Group")]
        public bool IsLocked { get; set; }

        [DisplayName("Moderate all topics in this Group")]
        public bool ModerateTopics { get; set; }

        [DisplayName("Moderate all posts in this Group")]
        public bool ModeratePosts { get; set; }

        [DisplayName("Sort Order")]
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; }

        [DisplayName("Parent Group")]
        public Guid? ParentGroup { get; set; }

        [DisplayName("Section")]
        public Guid? Section { get; set; }

        [Required]
        [DisplayName("Group Owner")]
        public Guid? GroupOwner { get; set; }

        [Required]
        [DisplayName("Group Administrators")]
        public IEnumerable<Guid> GroupAdministrators { get; set; }

        public List<SelectListItem> AllGroups { get; set; }

        public IEnumerable<SelectListItem> AllSections { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }

        public byte[] RowVersion { get; set; }

        [DisplayName("Group is public")]
        public bool Public { get; set; }


        [DisplayName("Page Title")]
        [MaxLength(80)]
        public string PageTitle { get; set; }

        [DisplayName("Meta Desc")]
        [MaxLength(200)]
        public string MetaDesc { get; set; }

        [DisplayName("Group Image")]
        public HttpPostedFileBase[] Files { get; set; }

        public string Image { get; set; }
    }

    public class DeleteGroupViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        public Group Group { get; set; }
        public List<Group> SubGroups { get; set; }
    }
}
using System.Collections.Generic;
using MvcForum.Core.Models.General;

namespace MvcForum.Web.ViewModels.Shared
{
    public class ActionMenuModel
    {
        public List<ActionLink> ActionLinks { get; set; }
        public Button ActionButton { get; set; }
    }
}
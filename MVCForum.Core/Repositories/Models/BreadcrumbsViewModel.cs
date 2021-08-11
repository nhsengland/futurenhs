//-----------------------------------------------------------------------
// <copyright file="BreadcrumbsViewModel.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a model to render breadcrumbs
    /// </summary>
    public class BreadcrumbsViewModel
    {
        /// <summary>
        /// List of linkable items (exclude last item).
        /// </summary>
        public List<BreadCrumbItem> BreadcrumbLinks { get; set; }

        /// <summary>
        /// Last itme in the list which should not be a link.
        /// </summary>
        public string LastEntry { get; set; }
    }
}
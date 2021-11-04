namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MvcForum.Core.Models.SystemPages;

    public interface ISystemPagesRepository
    {
        /// <summary>
        /// Gets All system Pages
        /// </summary>
        /// <returns>A list of System Pages</returns>
        Task<IEnumerable<SystemPageViewModel>> GetSystemPages(CancellationToken cancellationToken);

        /// <summary>
        /// Gets a specific page based on id
        /// </summary>
        /// <param name="id">Id of the page to return</param>
        /// <returns>A system page based on Id</returns>
        Task<SystemPageViewModel> GetSystemPageById(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a specific page based on Slug
        /// </summary>
        /// <param name="slug">Slug of the page to return</param>
        /// <returns>A system page based on Slug</returns>
        Task<SystemPageViewModel> GetSystemPageBySlug(string slug, CancellationToken cancellationToken);

    }
}

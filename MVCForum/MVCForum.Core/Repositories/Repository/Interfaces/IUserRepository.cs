namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Repositories.Models;

    public interface IUserRepository
    {
        Task<User> GetUser(string username, CancellationToken cancellationToken = default(CancellationToken));
    }
}

using MvcForum.Core.Models.SystemPages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Command.Interfaces
{
    public interface ISystemPagesCommand
    {
        Task<SystemPageWriteResponse> CreateSystemPage(SystemPageWriteViewModel page, CancellationToken cancellationToken);
        Task<SystemPageWriteResponse> UpdateSystemPage(SystemPageWriteViewModel page, CancellationToken cancellationToken);
        Task<SystemPageWriteResponse> DeleteSystemPage(Guid id, CancellationToken cancellationToken);
    }
}

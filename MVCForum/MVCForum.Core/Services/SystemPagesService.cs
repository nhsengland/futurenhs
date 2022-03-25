using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models.SystemPages;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Repository.Interfaces;

namespace MvcForum.Core.Services
{
    public class SystemPagesService : ISystemPagesService
    {
        private readonly ISystemPagesCommand _systemPagesCommand;
        private readonly ISystemPagesRepository _systemPagesRepository;

        public SystemPagesService(ISystemPagesCommand systemPagesCommand, ISystemPagesRepository systemPagesRepository)
        {
            _systemPagesCommand = systemPagesCommand;
            _systemPagesRepository = systemPagesRepository;
        }

        public async Task<SystemPageWriteResponse> CreateSystemPage(SystemPageWriteViewModel model, CancellationToken cancellationToken)
        {
            var response = await _systemPagesCommand.CreateSystemPage(model, cancellationToken);
            return response;
        }

        public async Task<SystemPageWriteResponse> UpdateSystemPage(SystemPageWriteViewModel model, CancellationToken cancellationToken)
        {
            var response = await _systemPagesCommand.UpdateSystemPage(model, cancellationToken);
            return response;
        }

        public async Task<SystemPageWriteResponse> DeleteSystemPage(Guid id, CancellationToken cancellationToken)
        {
            var response = await _systemPagesCommand.DeleteSystemPage(id, cancellationToken);
            return response;
        }

        public async Task<IEnumerable<SystemPageViewModel>> GetAllSystemPages(CancellationToken cancellationToken)
        {
            var systemPage = await _systemPagesRepository.GetSystemPages(cancellationToken);
            return systemPage;
        }

        public async Task<SystemPageViewModel> GetSystemPageById(Guid id, CancellationToken cancellationToken)
        {
            var systemPage = await _systemPagesRepository.GetSystemPageById(id, cancellationToken);
            return systemPage;
        }

        public async Task<SystemPageViewModel> GetSystemPageBySlug(string slug, CancellationToken cancellationToken)
        {
            var systemPage = await _systemPagesRepository.GetSystemPageBySlug(slug, cancellationToken);
            return systemPage;
        }

        public bool IsValidSlug(string slug)
        {
            const string regex = @"^[a-z\d](?:[a-z\d_-]*[a-z\d])?$";
            return Regex.IsMatch(slug, regex);
        }
    }
}

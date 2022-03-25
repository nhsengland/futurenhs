using MvcForum.Core.Interfaces;
using MvcForum.Core.Models.Entities;
using MvcForum.Core.Models.Enums;
using MvcForum.Core.Models.SystemPages;
using MvcForum.Core.Repositories.Command.Interfaces;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Command
{
    public class SystemPagesCommand : ISystemPagesCommand
    {
        private readonly IMvcForumContext _context;

        public SystemPagesCommand(IMvcForumContext context)
        {
            _context = context;
        }

        public async Task<SystemPageWriteResponse> CreateSystemPage(SystemPageWriteViewModel page, CancellationToken cancellationToken)
        {
            var response = new SystemPageWriteResponse();

            var systemPage = new SystemPage()
            {
                Title = page.Title,
                Content = page.Content,
                Slug = page.Slug,
                IsDeleted = false,
            };

            try
            {
                if (_context.SystemPage.Any(x => x.Slug == page.Slug && x.IsDeleted == false))
                {
                    response.Response = ResponseType.AlreadyExists;
                    return response;
                }
                _context.SystemPage.Add(systemPage);
                await _context.SaveChangesAsync(cancellationToken);

                response.Id = systemPage.Id;
                response.Response = ResponseType.Success;

            }
            catch (DbEntityValidationException)
            {
                response.Response = ResponseType.Error;
            }

            return response;
        }

        public async Task<SystemPageWriteResponse> UpdateSystemPage(SystemPageWriteViewModel page, CancellationToken cancellationToken)
        {
            var response = new SystemPageWriteResponse();
            try
            {
                var result = _context.SystemPage.FirstOrDefault(x => x.Id == page.Id && x.IsDeleted == false);

                if (result != null)
                {
                    if (_context.SystemPage.Any(x => x.Slug == page.Slug && x.IsDeleted == false && x.Id != page.Id))
                    {
                        response.Response = ResponseType.AlreadyExists;
                        return response;
                    }

                    result.Title = page.Title;
                    result.Content = page.Content;
                    result.Slug = page.Slug.ToLower();
                    await _context.SaveChangesAsync(cancellationToken);

                    response.Response = ResponseType.Success;
                    return response;
                }

                response.Response = ResponseType.DoesntExist;
                return response;
            }
            catch (DbEntityValidationException)
            {
                response.Response = ResponseType.Error;
            }

            return response;
        }

        public async Task<SystemPageWriteResponse> DeleteSystemPage(Guid id, CancellationToken cancellationToken)
        {
            var response = new SystemPageWriteResponse();
            try
            {
                var result = _context.SystemPage.FirstOrDefault(x => x.Id == id && x.IsDeleted == false);

                if (result != null)
                {
                    result.IsDeleted = true;
                    await _context.SaveChangesAsync(cancellationToken);

                    response.Response = ResponseType.Success;
                    return response;
                }

                response.Response = ResponseType.DoesntExist;
                return response;
            }
            catch (DbEntityValidationException)
            {
                response.Response = ResponseType.Error;
            }

            return response;
        }
    }
}

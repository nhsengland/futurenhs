using FutureNHS.Api.Models.Pagination.Filter;

namespace FutureNHS.Api.Models.Pagination.Services
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}

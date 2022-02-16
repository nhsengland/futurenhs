using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Wrappers;
using Microsoft.AspNetCore.WebUtilities;

namespace FutureNHS.Api.Models.Pagination.Helpers
{
    public static  class PaginationHelper
    {

        public static PagedResponse<T> CreatePagedResponse<T>(T pagedData, PaginationFilter validFilter, uint totalRecords, string route)
        {
            
            var response = new PagedResponse<T>(pagedData, validFilter.Offset, validFilter.Limit);
            var totalPages = totalRecords / (double)validFilter.Limit;
            var roundedTotalPages = Convert.ToUInt32(Math.Ceiling(totalPages));
            uint finalOffset = Convert.ToUInt32(Math.Floor(totalPages) * validFilter.Limit); ;

                response.NextPage =
                validFilter.Offset + validFilter.Limit < totalRecords
                    ? BuildRoute(new PaginationFilter(validFilter.Offset + validFilter.Limit, validFilter.Limit, validFilter.Sort), route)
                    : null;
            response.PreviousPage =
                validFilter.Offset != 0 && validFilter.Offset - validFilter.Limit < totalRecords
                    ? BuildRoute(new PaginationFilter(validFilter.Offset - validFilter.Limit, validFilter.Limit, validFilter.Sort), route)
                    : null;
            response.FirstPage = BuildRoute(new PaginationFilter(0, validFilter.Limit, validFilter.Sort), route);
            response.LastPage = BuildRoute(new PaginationFilter(finalOffset, validFilter.Limit, validFilter.Sort), route);
            response.TotalPages = roundedTotalPages;
            response.TotalRecords = totalRecords;
            return response;
        }

        private static string BuildRoute(PaginationFilter paginationFilter,string route)
        {
            var modifiedRoute = QueryHelpers.AddQueryString(route, "offset", paginationFilter.Offset.ToString());
            modifiedRoute = QueryHelpers.AddQueryString(modifiedRoute, "limit", paginationFilter.Limit.ToString());
            return modifiedRoute;
        }
    }
}

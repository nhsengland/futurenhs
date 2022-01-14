using FutureNHS.Api.Models.Pagination.Filter;
using FutureNHS.Api.Models.Pagination.Wrappers;
using Microsoft.AspNetCore.WebUtilities;

namespace FutureNHS.Api.Models.Pagination.Helpers
{
    public static class PaginationHelper
    {
        //public static PagedResponse<IEnumerable<T>> CreatePagedResponse<T>(IEnumerable<T> pagedData, PaginationFilter validFilter, int totalRecords, string route)
        //{
        //    var response = new PagedResponse<IEnumerable<T>>(pagedData, validFilter.Offset, validFilter.Limit);
        //    var totalPages = totalRecords / (double)validFilter.Limit;
        //    var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
        //    response.NextPage =
        //        validFilter.Offset >= 1 && validFilter.Offset < roundedTotalPages
        //        ? BuildRoute(new PaginationFilter(validFilter.Offset + 1, validFilter.Limit), route)
        //        : null;
        //    response.PreviousPage =
        //        validFilter.Offset - 1 >= 1 && validFilter.Offset <= roundedTotalPages
        //        ? BuildRoute(new PaginationFilter(validFilter.Offset - 1, validFilter.Limit), route)
        //        : null;
        //    response.FirstPage = BuildRoute(new PaginationFilter(1, validFilter.Limit), route);
        //    response.LastPage = BuildRoute(new PaginationFilter(roundedTotalPages, validFilter.Limit), route);
        //    response.TotalPages = roundedTotalPages;
        //    response.TotalRecords = totalRecords;
        //    return response;
        //}

        public static PagedResponse<IEnumerable<T>> CreatePagedResponse<T>(IEnumerable<T> pagedData, PaginationFilter validFilter, uint totalRecords, string route)
        {
            
            var response = new PagedResponse<IEnumerable<T>>(pagedData, validFilter.Offset, validFilter.Limit);
            var totalPages = totalRecords / (double)validFilter.Limit;
            var roundedTotalPages = Convert.ToUInt32(Math.Ceiling(totalPages));
            uint finalOffset = 0;
            while (finalOffset < roundedTotalPages)
            {
                if (finalOffset + validFilter.Limit < totalRecords)
                {
                    finalOffset += validFilter.Limit;
                }
                else
                {
                    break;
                }
            }
                response.NextPage =
                validFilter.Offset + validFilter.Limit < totalRecords
                    ? BuildRoute(new PaginationFilter(validFilter.Offset + validFilter.Limit, validFilter.Limit), route)
                    : null;
            response.PreviousPage =
                validFilter.Offset != 0 && validFilter.Offset <= roundedTotalPages
                    ? BuildRoute(new PaginationFilter(validFilter.Offset - validFilter.Limit, validFilter.Limit), route)
                    : null;
            response.FirstPage = BuildRoute(new PaginationFilter(0, validFilter.Limit), route);
            response.LastPage = BuildRoute(new PaginationFilter(finalOffset, validFilter.Limit), route);
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

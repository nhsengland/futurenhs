using System.ComponentModel.DataAnnotations;
using FutureNHS.Application.Application.HardCodedSettings;

namespace FutureNHS.Api.Models.Pagination.Filter
{
    public class PaginationFilter
    {
        [Range(PaginationSettings.MinPageNumber, int.MaxValue)]
        public int PageNumber { get; set; }
        [Range(PaginationSettings.MinPageSize, PaginationSettings.MaxPageSize)]
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            PageNumber = PaginationSettings.MinPageNumber;
            PageSize = PaginationSettings.DefaultPageSize;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;

        }
    }
}

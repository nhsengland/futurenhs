using System.ComponentModel.DataAnnotations;
using FutureNHS.Api.Application.Application.HardCodedSettings;

namespace FutureNHS.Api.Models.Pagination.Filter
{
    public sealed class PaginationFilter
    {
        [Range(PaginationSettings.MinOffset, int.MaxValue)]
        public uint Offset { get; set; }
        [Range(PaginationSettings.MinLimit, PaginationSettings.MaxLimit)]
        public uint Limit { get; set; }
        public string? Sort { get; set; }
        public PaginationFilter()
        {
            Offset = PaginationSettings.MinOffset;
            Limit = PaginationSettings.DefaultPageSize;
        }
        public PaginationFilter(uint offset, uint limit, string sort)
        {
            Offset = offset;
            Limit = limit;
            Sort = sort;
        }
    }
}

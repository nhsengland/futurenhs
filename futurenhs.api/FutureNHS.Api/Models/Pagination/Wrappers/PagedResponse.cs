namespace FutureNHS.Api.Models.Pagination.Wrappers
{
    public class PagedResponse<T> : Response<T>
    {
        public uint Offset { get; set; }
        public uint Limit { get; set; }
        public string? FirstPage { get; set; }
        public string? LastPage { get; set; }

        public uint TotalPages { get; set; }
        public uint TotalRecords { get; set; }
        public string? NextPage { get; set; }
        public string? PreviousPage { get; set; }

        public PagedResponse(T data, uint offset, uint limit)
        {
            Offset = offset;
            Limit = limit;
            Data = data;
        }
    }
}

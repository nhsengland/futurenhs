namespace FutureNHS.Api.DataAccess.Models.Search
{
    public record SearchResultTotalsByType
    {
        public int Groups { get; set; }
        public int Files { get; set; }
        public int Folders { get; set; }
        public int Comments { get; set; }
        public int Discussions { get; set; }
    }
}

using FutureNHS.Api.DataAccess.Models;

namespace FutureNHS.Api.Models.Folder
{
    public sealed record Folder : BaseData
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}

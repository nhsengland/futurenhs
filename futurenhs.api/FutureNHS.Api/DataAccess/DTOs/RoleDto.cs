using System.Diagnostics.Contracts;

namespace FutureNHS.Api.DataAccess.DTOs
{
    public sealed class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

    }
}

using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Providers.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateReadOnlyConnection();
        IDbConnection CreateWriteOnlyConnection();
    }
}

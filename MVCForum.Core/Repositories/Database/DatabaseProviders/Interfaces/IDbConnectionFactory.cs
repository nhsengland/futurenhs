namespace MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces
{
    using System.Data;

    public interface IDbConnectionFactory
    {
        IDbConnection CreateReadOnlyConnection();
        IDbConnection CreateWriteOnlyConnection();
    }
}

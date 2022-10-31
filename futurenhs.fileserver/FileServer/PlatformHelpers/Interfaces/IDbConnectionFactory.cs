using System.Data;

namespace FileServer.PlatformHelpers.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateReadOnlyConnection();
    IDbConnection CreateWriteOnlyConnection();
}
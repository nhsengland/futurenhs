//-----------------------------------------------------------------------
// <copyright file="DbConnectionFactory.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Database.DatabaseProviders
{
    using MvcForum.Core.Interfaces.Providers;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using MvcForum.Core.Repositories.Database.RetryPolicy;
    using System.Data;

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDbRetryPolicy _retryPolicy;
        private readonly IConfigurationProvider _configurationProvider;

        public DbConnectionFactory(IConfigurationProvider configurationProvider, IDbRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
            _configurationProvider = configurationProvider;
        }

        public IDbConnection CreateReadOnlyConnection()
        {
            return new ReliableSqlDbConnection(_configurationProvider.GetReadOnlyConnectionString(), _retryPolicy);
        }

        public IDbConnection CreateWriteConnection()
        {
            return new ReliableSqlDbConnection(_configurationProvider.GetWriteConnectionString(), _retryPolicy);
        }
    }
}

using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Domain;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class DomainDataProvider : IDomainDataProvider
    { 
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<DomainDataProvider> _logger;

        public DomainDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<DomainDataProvider> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<bool> IsDomainApprovedAsync(string emailDomain, CancellationToken cancellationToken = default)
        {
            const string query =
                @"SELECT 
                    CASE WHEN EXISTS 
                        (SELECT * 
                         FROM ApprovedDomain 
                         WHERE EmailDomain = @EmailDomain
                         AND IsDeleted = 0) 
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                    END";

            var queryDefinition = new CommandDefinition(query, new
            {
                EmailDomain = emailDomain.ToLower()
            }, cancellationToken: cancellationToken);
            
            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var domain = await dbConnection.QuerySingleAsync<bool>(queryDefinition);

            return domain;
        }
    }
}
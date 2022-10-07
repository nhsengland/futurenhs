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

        public async Task<WhitelistDomain> GetWhitelistedDomainsAsync(string emailDomain, CancellationToken cancellationToken = default)
        {
            const string query =
                @"SELECT 
                        wd.Id,    
                        wd.EmailDomain,
				    FROM WhitelistedDomain wd 
                    WHERE wd.IsDeleted = 0
                        AND wd.EmailDomain = @EmailDomain";

            var queryDefinition = new CommandDefinition(query, new
            {
                EmailDomain = emailDomain
            }, cancellationToken: cancellationToken);
            
            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var domain = await dbConnection.QuerySingleAsync<WhitelistDomain>(queryDefinition);

            return domain;
        }

        public Task<WhitelistDomain> GetWhitelistedDomainAsync(string emailDomain, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
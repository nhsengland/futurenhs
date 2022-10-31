using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
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
            var domainToCheck = emailDomain;
            var domainArr = domainToCheck.Split('.');
            var domainHasPrefix = domainArr.Length >= 3;
            if (domainHasPrefix)
            {
                domainToCheck = $"*.{domainArr[^2]}.{domainArr[^1]}";
            }
            const string query =
                @"SELECT 
                    CASE WHEN EXISTS 
                        (SELECT * 
                         FROM ApprovedDomain 
                         WHERE EmailDomain = @EmailDomain) 
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                    END";

            var queryDefinition = new CommandDefinition(query, new
            {
                EmailDomain = domainToCheck.ToLower()
            }, cancellationToken: cancellationToken);
            
            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var domain = await dbConnection.QuerySingleAsync<bool>(queryDefinition);

            return domain;
        } 
        public async Task<(uint, IEnumerable<ApprovedDomain>)> GetDomainsAsync(uint offset, uint limit, CancellationToken cancellationToken = default)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query =
                @$" SELECT
                                [{nameof(ApprovedDomain.Id)}]                   = Id,
                                [{nameof(ApprovedDomain.EmailDomain)}]          = EmailDomain

 
                    FROM        ApprovedDomain
                    ORDER BY    EmailDomain asc
                    OFFSET      @Offset ROWS
                    FETCH NEXT  @Limit ROWS ONLY;

                    SELECT      COUNT(*) 
                    FROM        ApprovedDomain;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit)
            });

            var domains = await reader.ReadAsync<ApprovedDomain>();

            var totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());
            
            return (totalCount, domains);
        }
  
    }
}
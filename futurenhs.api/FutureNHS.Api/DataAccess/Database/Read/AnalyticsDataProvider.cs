using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.Member;
using Microsoft.Extensions.Options;
using FutureNHS.Api.DataAccess.Models.Identity;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class AnalyticsDataProvider : IAnalyticsDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<AnalyticsDataProvider> _logger;
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;

        public AnalyticsDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<AnalyticsDataProvider> logger,
            IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<int> GetActiveUserCountAsync(DateTime startTime, DateTime endTime,
            CancellationToken cancellationToken = default)
        {
            const string query =
                @"  
                   SELECT
                   COUNT(DISTINCT MembershipUserId) AS 'activeUserCount'
                   FROM [FutureNHS].[dbo].[MembershipUserActivity]
                   WHERE LastActivityDateUTC BETWEEN @startTime AND @endTime
                   ";

            var queryDefinition = new CommandDefinition(query, new
            {
                startTime = startTime,
                endTime = endTime,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to get active user count for timeframe was not successful.",
                    queryDefinition);
                throw new ApplicationException("Error: User request to get active user count for timeframe was not successful.");
            }
            
            return result;
        }
    }
}

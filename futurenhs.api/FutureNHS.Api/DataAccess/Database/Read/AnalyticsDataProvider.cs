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
using MimeDetective.Storage.Xml.v2;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class AnalyticsDataProvider : IAnalyticsDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<AnalyticsDataProvider> _logger;

        public AnalyticsDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<AnalyticsDataProvider> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<ActiveUsers> GetActiveUserCountAsync(DateTime startTime, DateTime endTime,
            CancellationToken cancellationToken = default)
        {
            const string query =
                @"
                   SELECT
                   COUNT(DISTINCT MembershipUserId) AS 'activeUserCount'
                   FROM [FutureNHS].[dbo].[MembershipUserActivity]
                   WHERE LastActivityDateUTC BETWEEN @startTime AND @endTime
                   ";
            
            var dailyQueryDefinition = new CommandDefinition(query, new
            {
                startTime = startTime,
                endTime = endTime,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var dailyResult = await dbConnection.ExecuteAsync(dailyQueryDefinition);

            if (dailyResult != 1)
            {
                _logger.LogError("Error: User request to get active user count for timeframe was not successful.",
                    dailyQueryDefinition);
                throw new ApplicationException("Error: User request to get active user count for timeframe was not successful.");
            }

            var activeUsers = new ActiveUsers()
            {
                Daily = dailyResult,
            };
            return activeUsers;
        }
    }
}

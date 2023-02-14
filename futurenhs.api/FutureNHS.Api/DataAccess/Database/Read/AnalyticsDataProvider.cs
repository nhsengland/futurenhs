using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;

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

        public async Task<uint> GetActiveUserCountAsync(DateTime startTime, DateTime endTime,
            CancellationToken cancellationToken = default)
        {
            const string query =
                @" SELECT
                   COUNT(DISTINCT MembershipUserId) AS 'activeUserCount'
                   FROM [dbo].[MembershipUserActivity]
                   WHERE LastActivityDateUTC BETWEEN @startDate AND @endDate
                   ";
            
            var queryDefinition = new CommandDefinition(query, 
            new {
                startDate = DateTime.SpecifyKind(startTime, DateTimeKind.Utc),
                endDate = DateTime.SpecifyKind(endTime, DateTimeKind.Utc),
            }, cancellationToken: cancellationToken);
                

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.QuerySingleAsync<uint>(queryDefinition);

            if (result < 0)
            {
                _logger.LogError("Error: User request to get active user count for timeframe was not successful.",
                    queryDefinition);
                throw new ApplicationException("Error: User request to get active user count for timeframe was not successful.");
            }

            return result;
        }
    }
}

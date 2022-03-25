using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models.SystemPage;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class SystemPageDataProvider : ISystemPageDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<SystemPageDataProvider> _logger;

        public SystemPageDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<SystemPageDataProvider> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }
        public async Task<SystemPage?> GetSystemPageAsync(string systemPageSlug, CancellationToken cancellationToken)
        {
            const string query =
                @$"SELECT    
                       [{nameof(SystemPage.Id)}]          = Id,
		               [{nameof(SystemPage.Slug)}]        = Slug,
		               [{nameof(SystemPage.Title)}]       = Title,
		               [{nameof(SystemPage.Content)}]     = Content,
                       [{nameof(SystemPage.RowVersion)}]  = RowVersion

                FROM   [SystemPage]
                WHERE  Slug = @Slug
                AND    IsDeleted = 0;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var commandDefinition = new CommandDefinition(query, new
            {
                Slug = systemPageSlug,
            }, cancellationToken: cancellationToken);

            return await dbConnection.QuerySingleOrDefaultAsync<SystemPage>(commandDefinition);
        }
    }
}

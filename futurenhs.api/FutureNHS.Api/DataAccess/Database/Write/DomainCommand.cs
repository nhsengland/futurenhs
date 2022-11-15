using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models.Domain;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class DomainCommand : IDomainCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<DomainCommand> _logger;

        public DomainCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<DomainCommand> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger;
        }

        public async Task<DomainDto> GetDomainAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT
                                [{nameof(DomainDto.Id)}]                   = Id,
                                [{nameof(DomainDto.EmailDomain)}]          = EmailDomain,
                                [{nameof(DomainDto.RowVersion)}]           = RowVersion
 
                    FROM        ApprovedDomain
                    WHERE       Id = @Id";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Id = id
            });

            var domain = await reader.ReadSingleAsync<DomainDto>();

            if (domain is null)
            {
                _logger.LogError("Error: User request to get domain was unsuccessful.", query);
                throw new DataException("Error: User unable to get domain.");
            }

            return domain;
        }
        
        public async Task<DomainDto> GetDeletedDomainAsync(string emailDomain, CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT
                                [{nameof(DomainDto.Id)}]                   = Id,
                                [{nameof(DomainDto.EmailDomain)}]          = EmailDomain,
                                [{nameof(DomainDto.RowVersion)}]           = RowVersion
 
                    FROM        ApprovedDomain
                    WHERE       EmailDomain = @EmailDomain
                    AND         IsDeleted   = 1";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                EmailDomain = emailDomain
            });

            var domain = await reader.ReadSingleAsync<DomainDto>();

            if (domain is null)
            {
                _logger.LogError("Error: User request to get domain was unsuccessful.", query);
                throw new DataException("Error: User unable to get domain.");
            }

            return domain;
        }

        
        public async Task<(uint, IEnumerable<ApprovedDomain>)> GetDomainsAsync(uint offset, uint limit, CancellationToken cancellationToken = default)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string domainsQuery = "WHERE isDeleted = 0";
            const string query =
                @$" SELECT
                                [{nameof(ApprovedDomain.Id)}]                   = Id,
                                [{nameof(ApprovedDomain.EmailDomain)}]          = EmailDomain,
                                [{nameof(ApprovedDomain.RowVersion)}]           = RowVersion
 
                    FROM        ApprovedDomain
                    {domainsQuery}
                    ORDER BY    EmailDomain asc
                    OFFSET      @Offset ROWS
                    FETCH NEXT  @Limit ROWS ONLY;

                    SELECT      COUNT(*) 
                    FROM        ApprovedDomain
                    {domainsQuery};";

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
        
        

        public async Task CreateApprovedDomainAsync(DomainDto emailDomain, CancellationToken cancellationToken)
        {
            try
            {
                const string query =
                  @"INSERT 
                    INTO [dbo].[ApprovedDomain] 
                        ([EmailDomain])
	                VALUES 
                        (@EmailDomain)";

                var queryDefinition = new CommandDefinition(query, new
                {
                    EmailDomain = emailDomain.EmailDomain,
                }, cancellationToken: cancellationToken);

                using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

                var result = await dbConnection.ExecuteAsync(queryDefinition);
                if (result != 1)
                {
                    _logger.LogError("Error: User request to add domain  was unsuccessful.", queryDefinition);
                    throw new DataException("Error: User unable to add domain.");
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error: User request to create was not successful.");
                throw new DataException("Error: User request to add domain was not successful.");
            }
        }

        public async Task DeleteApprovedDomainAsync(DomainDto emailDomain, CancellationToken cancellationToken)
        {
            try
            {
                const string query =

                    @"  
	                UPDATE  [dbo].[ApprovedDomain]
                    SET     isDeleted               = 1
                    WHERE         
                            [Id]                    = @Id
                    AND 
                            [RowVersion]            = @RowVersion";

                var queryDefinition = new CommandDefinition(query, new
                {
                    Id = emailDomain.Id,
                    RowVersion = emailDomain.RowVersion,
                }, cancellationToken: cancellationToken);

                using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

                var result = await dbConnection.ExecuteAsync(queryDefinition);

                if (result != 1)
                {
                    _logger.LogError("Error: User request to delete was not successful.", queryDefinition);
                    throw new DBConcurrencyException("Error: User request to delete was not successful.");
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error: User request to delete was not successful.");
                throw new DBConcurrencyException("Error: User request was not successful.");
            }
        }
        
        public async Task RestoreApprovedDomainAsync(DomainDto emailDomain, CancellationToken cancellationToken)
        {
            try
            {
                const string query =

                    @"  
	                UPDATE  [dbo].[ApprovedDomain]
                    SET     isDeleted               = 0
                    WHERE         
                            [Id]                    = @Id
                    AND 
                            [RowVersion]            = @RowVersion";

                var queryDefinition = new CommandDefinition(query, new
                {
                    Id = emailDomain.Id,
                    RowVersion = emailDomain.RowVersion,
                }, cancellationToken: cancellationToken);

                using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

                var result = await dbConnection.ExecuteAsync(queryDefinition);

                if (result != 1)
                {
                    _logger.LogError("Error: User request to delete was not successful.", queryDefinition);
                    throw new DBConcurrencyException("Error: User request to delete was not successful.");
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error: User request to delete was not successful.");
                throw new DBConcurrencyException("Error: User request was not successful.");
            }
        }

    }
}

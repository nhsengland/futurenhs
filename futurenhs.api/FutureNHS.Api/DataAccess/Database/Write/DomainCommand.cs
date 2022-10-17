using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;
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
                                [{nameof(DomainDto.EmailDomain)}]          = EmailDomain
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
	                DELETE 
                    FROM    [dbo].[ApprovedDomain]
                    WHERE         
                            [Id] = @Id
                    AND 
                            [RowVersion] = @RowVersion";

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

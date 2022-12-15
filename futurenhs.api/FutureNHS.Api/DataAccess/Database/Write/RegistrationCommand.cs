using Dapper;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.DataAccess.Models.Registration;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class RegistrationCommand : IRegistrationCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<GroupCommand> _logger;
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;

        public RegistrationCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<GroupCommand> logger,
            IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task DeletePlatformInvite(Guid inviteId, byte[] rowVersion,
            CancellationToken cancellationToken = default)
        {
            {
                const string query =
                    @$"
                    DELETE          
                    FROM                [dbo].[PlatformInvite]
                    WHERE 
                                    [Id]            = @InviteId
                    AND             [RowVersion]    = @RowVersion";

                var queryDefinition = new CommandDefinition(query, new
                {
                    InviteId = inviteId,
                    RowVersion = rowVersion
                }, cancellationToken: cancellationToken);

                using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

                var result = await dbConnection.ExecuteAsync(queryDefinition);

                if (result != 1)
                {
                    _logger.LogError($"Error: Unable to update platform invite:{0} ", inviteId);
                    throw new DBConcurrencyException("Error: Unable to update platform invite");
                }
            }
        }
        
        public async Task<PlatformInvite> GetPlatformInviteById(Guid inviteId, CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT
                                [{nameof(PlatformInvite.Id)}]                            = Id,
                                [{nameof(PlatformInvite.RowVersion)}]                    = RowVersion


                    FROM        [PlatformInvite] pi
                    WHERE         pi.Id = @Id;";

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var commandDefinition = new CommandDefinition(query, new
            {
                Id = inviteId,
            }, cancellationToken: cancellationToken);

            return await dbConnection.QuerySingleOrDefaultAsync<PlatformInvite>(commandDefinition);
        }

    }

}

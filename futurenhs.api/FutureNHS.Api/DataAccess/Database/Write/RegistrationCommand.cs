// using Dapper;
// using FutureNHS.Api.Configuration;
// using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
// using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
// using FutureNHS.Api.DataAccess.DTOs;
// using FutureNHS.Api.DataAccess.Models;
// using FutureNHS.Api.DataAccess.Models.Group;
// using FutureNHS.Api.Exceptions;
// using Microsoft.Data.SqlClient;
// using Microsoft.Extensions.Options;
// using System.Data;
// using FutureNHS.Api.Application.Application.HardCodedSettings;
// using FutureNHS.Api.DataAccess.Models.Registration;
//
// namespace FutureNHS.Api.DataAccess.Database.Write
// {
//     public class RegistrationCommand : IRegistrationCommand
//     {
//         private readonly IAzureSqlDbConnectionFactory _connectionFactory;
//         private readonly ILogger<GroupCommand> _logger;
//         private readonly IOptions<AzureImageBlobStorageConfiguration> _options;
//
//         public RegistrationCommand(IAzureSqlDbConnectionFactory connectionFactory, ILogger<GroupCommand> logger, IOptions<AzureImageBlobStorageConfiguration> options)
//         {
//             _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//             _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
//             _options = options ?? throw new ArgumentNullException(nameof(options));
//         }
// //
// //         public async Task<(uint totalCount, IEnumerable<PlatformInvite>)> GetPlatformInvitesAsync(Guid userId, uint offset, uint limit, 
// //             CancellationToken cancellationToken = default)
// //         {
// //             
// //             if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
// //             {
// //                 throw new ArgumentOutOfRangeException(nameof(limit));
// //             }
// //
// //             uint totalCount;
// //
// //             IEnumerable<PlatformInvite> invites;
// //             
// //                         
// //             var inviteQuery = "WHERE IsDeleted = 0";
// //
// //             string query =
// //                 @$"SELECT 
// //                     [{nameof(PlatformInvite.Id)}]                               = Id,
// //                     [{nameof(PlatformInvite.GroupId)}]                          = GroupId,
// //                     [{nameof(PlatformInvite.Email)}]                            = Email,
// //                     [{nameof(PlatformInvite.RowVersion)}]                       = RowVersion,
// //                     [{nameof(PlatformInvite.CreatedAtUTC)}]                     = CreatedAtUTC,
// //     
// //                 FROM PlatformInvite            
// //                 {inviteQuery}
// //                 ORDER BY CreatedAtUTC
// //                 OFFSET @Offset ROWS
// //                 FETCH NEXT @Limit ROWS ONLY;
// //
// //                 SELECT COUNT(*) FROM PlatformInvite
// //                 {inviteQuery}";
// //             
// //             using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
// //             {
// //                 using var reader = await dbConnection.QueryMultipleAsync(query, new
// //                 {
// //                     Offset = Convert.ToInt32(offset),
// //                     Limit = Convert.ToInt32(limit),
// //                     UserId = userId
// //                 });
// //
// //                 invites = reader.Read<PlatformInvite>().ToList();
// //                 totalCount = await reader.ReadFirstAsync<uint>();
// //
// //             }
// //
// //             return (totalCount, invites);
// //         }
// //         
// //         public async Task<IEnumerable<PendingGroupMember>> GetPendingGroupInvitesAsync(Guid groupId, CancellationToken cancellationToken = default)
// //         {
// //             
// //             IEnumerable<PendingGroupMember> members;
// //             
// //                         
// //             var inviteQuery = "WHERE IsDeleted = 0 AND GroupId = @GroupId";
// //
// //             string query =
// //                 @$"SELECT 
// //                     [{nameof(PlatformInvite.Id)}]                               = Id,
// //                     [{nameof(PlatformInvite.GroupId)}]                          = GroupId,
// //                     [{nameof(PlatformInvite.Email)}]                            = Email,
// //                     [{nameof(PlatformInvite.RowVersion)}]                       = RowVersion,
// //                     [{nameof(PlatformInvite.CreatedAtUTC)}]                     = CreatedAtUTC,
// //     
// //                 FROM PlatformInvite            
// //                 {inviteQuery}
// //                 ORDER BY CreatedAtUTC";
// //             
// //             using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
// //             {
// //                 using var reader = await dbConnection.QueryMultipleAsync(query, new
// //                 {
// //                     GroupId = groupId
// //                 });
// //
// //                 var invites = reader.Read<PlatformInvite>().ToList();
// //
// //                 members = invites.Select(p => new PendingGroupMember()
// //                 {
// //                     Id = null,
// //                     Email = p.Email,
// //                     GroupInvite = null,
// //                     PlatformInvite = p,
// //
// //                 });
// //             }
// //
// //             return members;
// //         }
// //         
//     }
// }

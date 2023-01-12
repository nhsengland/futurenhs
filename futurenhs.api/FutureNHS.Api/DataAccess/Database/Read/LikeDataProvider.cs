using Dapper;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Exceptions;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class LikeDataProvider : ILikeDataProvider
    {

        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<ImageDataProvider> _logger;
        
        public LikeDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<ImageDataProvider> logger)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public async Task<EntityLikeData> GetEntityLikesAsync(Guid entityId, CancellationToken cancellationToken = default)
        {
            const string query =
               @$"SELECT
                                [{nameof(EntityLikeData.Id)}]            = entityLike.Entity_Id,         
                                [{nameof(EntityLikeData.CreatedAtUtc)}]        = entityLike.CreatedAtUTC,
                                [{nameof(EntityLikeData.MembershipUserId)}]    = entityLike.MembershipUser_Id,

                    FROM            Entity_Like entityLike
					WHERE           LikedEntity.Entity_Id = @EntityId";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var likedEntityData = await dbConnection.QueryFirstOrDefaultAsync<EntityLikeData>(query, new
            {
                EntityId = entityId,
            });

            if (likedEntityData is null)
            {
                _logger.LogError($"Not Found: Like record for entity:{0} not found", entityId);
                throw new NotFoundException("Not Found: Like record for entity not found");
            }

            return likedEntityData;
        }
        
        public async Task<IEnumerable<EntityLikeData>> GetEntityLikeListAsync(Guid entityId, CancellationToken cancellationToken = default)
        {
            const string query =
                @$"SELECT
                                [{nameof(EntityLikeData.Id)}]            = entityLike.Entity_Id,         
                                [{nameof(EntityLikeData.MembershipUserId)}]    = entityLike.MembershipUser_Id,
                                [{nameof(Models.Shared.Properties.AtUtc)}]     = entityLike.CreatedAtUTC,
                                [{nameof(UserNavProperty.Id)}]                 = CreatedByUser.Id,
                                [{nameof(UserNavProperty.Name)}]               = CreatedByUser.FirstName + ' ' + CreatedByUser.Surname,
                                [{nameof(UserNavProperty.Slug)}]               = CreatedByUser.Slug
                    FROM            Entity_Like entityLike
                    LEFT JOIN       MembershipUser CreatedByUser ON entityLike.MembershipUser_Id = CreatedByUser.Id
					WHERE           entityLike.Entity_Id = @EntityId";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var likedEntityData = await dbConnection.QueryAsync<EntityLikeData, Models.Shared.Properties, UserNavProperty, EntityLikeData>(query,
                (likeDetails, likeProperties, userNavProperty) =>
            {
                if (likeProperties is not null)
                {
                    if (userNavProperty is not null)
                    {
                        var likeWithUserInfo = likeDetails with { FirstRegistered = likeProperties with { By = userNavProperty } };
                        return likeWithUserInfo;
                    }
                    likeDetails = likeDetails with { FirstRegistered = likeProperties };
                    return likeDetails;
                }

                return likeDetails;

            }, new
            {
                EntityId = entityId,
            }, splitOn: $"{nameof(EntityLikeData.Id)}");

            return likedEntityData;
        }
    }
}

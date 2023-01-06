﻿using Dapper;
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

        public async Task<EntityLikeData> GetEntityLikesAsync(Guid entityId, CancellationToken cancellationToken = default)
        {
            const string query =
               @$"SELECT
                                [{nameof(EntityLikeData.EntityId)}]            = entityLike.Entity_Id,         
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
                                [{nameof(EntityLikeData.EntityId)}]            = entityLike.Entity_Id,         
                                [{nameof(EntityLikeData.CreatedAtUtc)}]        = entityLike.CreatedAtUTC,
                                [{nameof(EntityLikeData.MembershipUserId)}]    = entityLike.MembershipUser_Id,
                                [{nameof(Models.Shared.Properties.AtUtc)}]     = folders.CreatedAtUtc,
                                [{nameof(UserNavProperty.Id)}]                 = CreatedByUser.Id,
                                [{nameof(UserNavProperty.Name)}]               = CreatedByUser.FirstName + ' ' + CreatedByUser.Surname,
                                [{nameof(UserNavProperty.Slug)}]               = CreatedByUser.Slug
                    FROM            Entity_Like entityLike
                    LEFT JOIN       MembershipUser CreatedByUser ON entityLike.MembershipUser_Id = CreatedByUser.Id
					WHERE           LikedEntity.Entity_Id = @EntityId";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var likedEntityData = await dbConnection.QueryMultipleAsync(query, new
            {
                EntityId = entityId,
            });
            
            var likes = likedEntityData.Read<EntityLikeData, Models.Shared.Properties, UserNavProperty, EntityLikeData>(
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

                }, splitOn: $"{nameof(Models.Shared.Properties.AtUtc)}, {nameof(EntityLikeData.EntityId)}");

            return likes;
        }
    }
}

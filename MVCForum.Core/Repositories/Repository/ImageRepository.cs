using Dapper;
using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
using MvcForum.Core.Repositories.Models;
using MvcForum.Core.Repositories.Repository.Interfaces;
using System;

namespace MvcForum.Core.Repositories.Repository
{
    public sealed class ImageRepository : IImageRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ImageRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public ImageViewModel Get(Guid imageId)
        {
            if (Guid.Empty == imageId) throw new ArgumentOutOfRangeException(nameof(imageId));

            const string query =
                @"
                    SELECT
                      [FileName],
                      [FileSizeBytes],
                      [Height],
                      [Width],
                      [MediaType],
                      [IsDeleted]
                    FROM
                      [dbo].[Image]
                    WHERE
                      Id = @imageId;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                imageId
            });

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return dbConnection.QueryFirstOrDefault<ImageViewModel>(commandDefinition);
            }
        }

        public Guid? GetGroupImageId(Guid groupId)
        {
            if (Guid.Empty == groupId) throw new ArgumentOutOfRangeException(nameof(groupId));

            const string query =
                @"
                    SELECT
                      [ImageId]
                    FROM
                      [dbo].[Group]
                    WHERE
                      Id = @groupId;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                groupId
            });

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return dbConnection.QuerySingleOrDefault<Guid?>(commandDefinition);
            }
        }

        public Guid? GetMembershipUserImageId(Guid membershipUserId)
        {
            if (Guid.Empty == membershipUserId) throw new ArgumentOutOfRangeException(nameof(membershipUserId));

            const string query =
                @"
                    SELECT
                      [ImageId]
                    FROM
                      [dbo].[MembershipUser]
                    WHERE
                      Id = @membershipUserId;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                membershipUserId
            });

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                return dbConnection.QuerySingleOrDefault<Guid?>(commandDefinition);
            }
        }
    }
}

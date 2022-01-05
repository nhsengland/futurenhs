using Dapper;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
using MvcForum.Core.Repositories.Models;
using MvcForum.Core.Utilities;
using System;

namespace MvcForum.Core.Repositories.Command
{
    public sealed class ImageCommand : IImageCommand
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ImageCommand(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public Guid Create(ImageViewModel image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));

            var imageId = GuidComb.GenerateComb();

            const string query =
                @"
                    INSERT INTO [dbo].[Image] 
                    (
                        [Id],
                        [FileName],
                        [FileSizeBytes],
                        [Height],
                        [Width],
                        [MediaType],
                        [IsDeleted]
                    )
                    VALUES 
                    (
	                    @imageId,
						@fileName,
						@fileSizeBytes,
						@height,
						@width,
						@mediaType,
                        0
                    );
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                imageId,
                fileName = image.FileName,
                fileSizeBytes = image.FileSizeBytes,
                height = image.Height,
                width = image.Width,
                mediaType = image.MediaType
            });

            using (var dbConnection = _connectionFactory.CreateWriteOnlyConnection())
            {
                var affectedRows = dbConnection.Execute(commandDefinition);

                if (affectedRows == 1)
                {
                    return imageId;
                }
            }
            return Guid.Empty;
        }

        public bool Update(ImageViewModel image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (Guid.Empty == image.Id) throw new ArgumentOutOfRangeException(nameof(image.Id));
                        
            const string query =
                @"
                    UPDATE [dbo].[Image] 
                    SET
                        [FileName] = @fileName,
                        [FileSizeBytes] = @fileSizeBytes,
                        [Height] = @height,
                        [Width] = @width,
                        [MediaType] = @mediaType
                    WHERE 
                        [Id] = @imageId;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                imageId = image.Id,
                fileName = image.FileName,
                fileSizeBytes = image.FileSizeBytes,
                height = image.Height,
                width = image.Width,
                mediaType = image.MediaType
            });

            using (var dbConnection = _connectionFactory.CreateWriteOnlyConnection())
            {
                var affectedRows = dbConnection.Execute(commandDefinition);

                if (affectedRows == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool UpdateGroupImageId(Guid groupId, Guid imageId)
        {
            if (Guid.Empty == groupId) throw new ArgumentOutOfRangeException(nameof(groupId));
            if (Guid.Empty == imageId) throw new ArgumentOutOfRangeException(nameof(imageId));


            const string query =
                @"
                    UPDATE [dbo].[Group]
                    SET ImageId = @imageId
                    WHERE Id = @groupId;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                groupId,
                imageId
            });

            using (var dbConnection = _connectionFactory.CreateWriteOnlyConnection())
            {
                var affectedRows = dbConnection.Execute(commandDefinition);

                if (affectedRows == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool UpdateMembershipUserImageId(Guid membershipUserId, Guid imageId)
        {
            if (Guid.Empty == membershipUserId) throw new ArgumentOutOfRangeException(nameof(membershipUserId));
            if (Guid.Empty == imageId) throw new ArgumentOutOfRangeException(nameof(imageId));

            const string query =
                @"
                    UPDATE [dbo].[MembershipUser]
                    SET ImageId = @imageId
                    WHERE Id = @membershipUserId;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                membershipUserId,
                imageId
            });

            using (var dbConnection = _connectionFactory.CreateWriteOnlyConnection())
            {
                var affectedRows = dbConnection.Execute(commandDefinition);

                if (affectedRows == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }    
}


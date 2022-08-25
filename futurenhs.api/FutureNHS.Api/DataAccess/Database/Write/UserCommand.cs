using Dapper;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Identity;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.Models.Member;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace FutureNHS.Api.DataAccess.Database.Write
{
    public class UserCommand : IUserCommand
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<UserCommand> _logger;
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;

        public UserCommand(IAzureSqlDbConnectionFactory connectionFactory,
            ILogger<UserCommand> logger,
            IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task CreateInviteUserAsync(GroupInviteDto entityLike, CancellationToken cancellationToken)
        {
            try
            {
                const string query =

                    @"  
	                INSERT INTO  [dbo].[GroupInvite]
                                 ([EmailAddress]
                                 ,[GroupId]
                                 ,[CreatedAtUTC]
                                 ,[ExpiresAtUTC])
                    VALUES
                                 (@EmailAddress
                                 ,@GroupId
                                 ,@CreatedAtUTC
                                 ,@ExpiresAtUTC)";


                var queryDefinition = new CommandDefinition(query, new
                {
                    EmailAddress = entityLike.EmailAddress,
                    GroupId = entityLike.GroupId,
                    CreatedAtUTC = entityLike.CreatedAtUTC,
                    ExpiresAtUTC = entityLike.ExpiresAtUTC
                }, cancellationToken: cancellationToken);

                using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

                var result = await dbConnection.ExecuteAsync(queryDefinition);
                if (result != 1)
                {
                    _logger.LogError("Error: CreateInviteUserAsync - User request to create was not successful.", queryDefinition);
                    throw new DBConcurrencyException("Error: User request was not successful.");
                }

            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error: CreateInviteUserAsync - User request to create was not successful.");
                throw new DBConcurrencyException("Error: User request was not successful.");
            }
        }

        public async Task<MemberProfile> GetMemberAsync(Guid id, CancellationToken cancellationToken)
        {
            const string query =
                @$" SELECT
                                [{nameof(MemberProfile.Id)}]                = member.Id,
                                [{nameof(MemberProfile.FirstName)}]         = member.FirstName,
                                [{nameof(MemberProfile.LastName)}]          = member.Surname,
                                [{nameof(MemberProfile.Pronouns)}]          = member.Pronouns,
                                [{nameof(MemberProfile.ImageId)}]           = member.ImageId,
                                [{nameof(MemberProfile.RoleId)}]            = memberInRole.RoleIdentifier,
                                [{nameof(MemberProfile.RowVersion)}]        = member.RowVersion,
                                [{nameof(ImageData.Id)}]                    = image.Id,
                                [{nameof(ImageData.Height)}]                = image.Height,
                                [{nameof(ImageData.Width)}]                 = image.Width,
                                [{nameof(ImageData.FileName)}]              = image.FileName,
                                [{nameof(ImageData.MediaType)}]             = image.MediaType
				    
            FROM        [MembershipUser] member
            JOIN		MembershipUsersInRoles memberInRole
            ON			memberInRole.UserIdentifier = member.Id
            LEFT JOIN   Image image
            ON          image.Id = member.ImageId
            WHERE       
                        member.[Id] = @Id";


            var queryDefinition = new CommandDefinition(query, new
            {
                Id = id,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryAsync<MemberProfile, Image, MemberProfile>(query,
                (group, image) =>
                {
                    if (image is not null)
                    {
                        var groupWithImage = @group with { Image = new ImageData(image, _options) };

                        return groupWithImage;
                    }

                    return @group;

                }, new
                {
                    Id = id,
                }, splitOn: "id");

            var memberProfile = reader.FirstOrDefault() ?? throw new NotFoundException("Member not found.");

            return memberProfile;
        }

        public async Task<MemberRole> GetMembershipUsersInRoleAsync(Guid userId, CancellationToken cancellationToken)
        {
            const string query =
                @$" SELECT 
                                [{nameof(MemberRole.MembershipUserId)}]  = muir.UserIdentifier, 
                                [{nameof(MemberRole.RoleId)}]            = muir.RoleIdentifier,
                                [{nameof(MemberRole.RowVersion)}]        = muir.RowVersion
				    
                    FROM        [MembershipUsersInRoles] muir
                    WHERE      
                                UserIdentifier = @UserId";

            var queryDefinition = new CommandDefinition(query, new
            {
                UserId = userId
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            return await dbConnection.QuerySingleOrDefaultAsync<MemberRole>(queryDefinition);
        }

        public async Task UpdateUserAsync(MemberDto user, byte[] rowVersion, CancellationToken cancellationToken)
        {
            const string query =
                @$" UPDATE        [dbo].[MembershipUser]
                    SET 
                                  [FirstName]      = @FirstName
                                 ,[Surname]        = @LastName
                                 ,[Pronouns]       = @Pronouns
                                 ,[ImageId]        = @ImageId
                                 ,[ModifiedAtUTC]  = @ModifiedAtUtc
                    
                    WHERE 
                                 [Id] = @Id
                    AND          [RowVersion] = @RowVersion";

            var queryDefinition = new CommandDefinition(query, new
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.Surname,
                Pronouns = user.Pronouns,
                ImageId = user.ImageId,
                ModifiedAtUTC = user.ModifiedAtUTC,
                RowVersion = rowVersion,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to edit user was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to edit user was not successful");
            }
        }

        public async Task UpdateUserRoleAsync(MemberRoleUpdate memberRoleUpdate, byte[] rowVersion, CancellationToken cancellationToken)
        {
            const string query =
                @$" UPDATE       [dbo].[MembershipUsersInRoles]
                    SET 
                                 [RoleIdentifier]   = @NewRoleId                    
                    WHERE 
                                 [UserIdentifier]   = @UserId
                    AND          [RoleIdentifier]   = @CurrentRoleId
                    AND          [RowVersion]       = @RowVersion";

            var queryDefinition = new CommandDefinition(query, new
            {
                UserId = memberRoleUpdate.MembershipUserId,
                NewRoleId = memberRoleUpdate.NewRoleId,
                CurrentRoleId = memberRoleUpdate.CurrentRoleId,
                RowVersion = rowVersion
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var result = await dbConnection.ExecuteAsync(queryDefinition);

            if (result != 1)
            {
                _logger.LogError("Error: User request to edit user role was not successful", queryDefinition);
                throw new DBConcurrencyException("Error: User request to edit user role was not successful");
            }
        }

        public async Task<UserDto> GetUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            const string query =
                @$" SELECT
                                [{nameof(UserDto.Id)}]                  = membershipUser.Id,
                                [{nameof(UserDto.UserName)}]            = membershipUser.UserName, 
                                [{nameof(UserDto.Email)}]               = membershipUser.Email,
                                [{nameof(UserDto.CreatedAtUtc)}]        = FORMAT(membershipUser.CreatedAtUTC, 'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(UserDto.ModifiedAtUtc)}]       = FORMAT(membershipUser.ModifiedAtUTC, 'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(UserDto.LastLoginDateUtc)}]    = FORMAT(membershipUser.LastLoginDateUTC,'yyyy-MM-ddTHH:mm:ssZ'),
                                [{nameof(UserDto.Slug)}]                = membershipUser.Slug,
                                [{nameof(UserDto.FirstName)}]           = membershipUser.FirstName,
                                [{nameof(UserDto.LastName)}]            = membershipUser.Surname,
                                [{nameof(UserDto.Initials)}]            = membershipUser.Initials,
                                [{nameof(ImageData.Id)}]		        = [image].Id,
                                [{nameof(ImageData.Height)}]	        = [image].Height,
                                [{nameof(ImageData.Width)}]		        = [image].Width,
                                [{nameof(ImageData.FileName)}]	        = [image].FileName,
                                [{nameof(ImageData.MediaType)}]         = [image].MediaType


                    FROM        [MembershipUser] membershipUser
                    LEFT JOIN   Image [image]
                    ON          [image].Id = membershipUser.ImageId   
                    WHERE       
                                membershipUser.Id = @UserId;";

            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);

            var userProfile = await dbConnection.QueryAsync<UserDto, Image, UserDto>(query,
                (user, image) =>
                {
                    if (image is not null)
                    {
                        return @user with { Image = new ImageData(image, _options) };
                    }
                    return @user;
                }, new
                {
                    UserId = userId
                }, splitOn: "id");

            return userProfile.SingleOrDefault() ?? throw new NotFoundException("User profile not found."); ;
        }

        public async Task<(uint totalCount, IEnumerable<MemberSearchDetails>)> SearchUsers(string term, uint offset, uint limit, string sort, CancellationToken cancellationToken)
        {
            const string query =
                @$"SELECT    
                                    [{nameof(MemberSearchDetails.Id)}] = mu.[Id],
                                    [{nameof(MemberSearchDetails.Username)}] = [UserName],
                                    [{nameof(MemberSearchDetails.Email)}] = [Email],
                                    [{nameof(MemberSearchDetails.FirstName)}] = [FirstName],
                                    [{nameof(MemberSearchDetails.LastName)}] = [Surname],
                                    [{nameof(MemberSearchDetails.Initials)}] = [Initials],
	                                [{nameof(MemberSearchDetails.Role)}] = mr.[RoleName]
                    
                    FROM			[dbo].[MembershipUser] mu
                    JOIN			MembershipUsersInRoles mir 
                    ON			    mir.UserIdentifier = mu.Id 	
                    JOIN			MembershipRole mr 
                    ON			    mr.Id = mir.RoleIdentifier	
                    WHERE			Email		                LIKE @Term
                    OR			    UserName	                LIKE @Term
                    OR			    FirstName	                LIKE @Term
                    OR			    Surname		                LIKE @Term
                    OR			    FirstName + ' ' + Surname   LIKE @Term
                    AND			    mu.IsDeleted = 0
                    AND			    mu.IsApproved = 1
                    AND			    mu.IsBanned = 0
                    ORDER BY        FirstName ASC
                    OFFSET          @Offset ROWS 
					FETCH NEXT		@Limit ROWS ONLY;

                    SELECT          COUNT(*)
                    FROM			[dbo].[MembershipUser] mu
                    WHERE			Email		                LIKE @Term
                    OR			    UserName	                LIKE @Term
                    OR			    FirstName	                LIKE @Term
                    OR			    Surname		                LIKE @Term
                    OR			    FirstName + ' ' + Surname   LIKE @Term
                    AND			    mu.IsDeleted = 0
                    AND			    mu.IsApproved = 1
                    AND			    mu.IsBanned = 0";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit),
                Term = $"%{term}%"
            });

            var results = reader.Read<MemberSearchDetails>();

            var total = Convert.ToUInt32(await reader.ReadFirstAsync<int>());

            return (total, results);
        }

        public async Task<Guid> RegisterUserAsync(MemberDto user, string subjectId, string issuer, string defaultRole, CancellationToken cancellationToken)
        {
            using var dbConnection = await _connectionFactory.GetReadWriteConnectionAsync(cancellationToken);
            await using var connection = new SqlConnection(dbConnection.ConnectionString);

            await connection.OpenAsync(cancellationToken);
            await using var transaction = connection.BeginTransaction();

            try
            {
                const string memberInsertquery =
                    @$" INSERT INTO   [dbo].[MembershipUser]

                                 ([Email]
                                 ,[UserName]
                                 ,[FirstName]
                                 ,[Surname]
                                 ,[Pronouns]
                                 ,[CreatedAtUTC]
                                 ,[ModifiedAtUTC]
                                 ,[IsApproved]
                                 ,[IsLockedOut]
                                 ,[IsBanned]
                                 ,[Slug]
                                 ,[HasAgreedToTermsAndConditions]
                                 )
                    OUTPUT       INSERTED.[Id]

                    VALUES
                                 (@Email
                                  ,@Email
                                  ,@FirstName
                                  ,@LastName
                                  ,@Pronouns
                                  ,@CreatedAtUTC
                                  ,@CreatedAtUTC
                                  ,1
                                  ,0
                                  ,0
                                  ,@Email
                                  ,@AgreedToTerms
                                 )";

                var insertMemberResult = await connection.ExecuteScalarAsync<Guid?>(memberInsertquery, new
                {
                    FirstName = user.FirstName,
                    LastName = user.Surname,
                    Pronouns = user.Pronouns,
                    CreatedAtUtc = user.CreatedAtUTC,
                    Email = user.Email,
                    AgreedToTerms = user.AgreedToTerms

                }, transaction: transaction);


                const string identityInsertquery =
                    @$" INSERT INTO   [dbo].[Identity]
                                  
                                 ([MembershipUser_Id]
                                 ,[Subject_Id]
                                 ,[Issuer]
                                 )
                    VALUES
                                 (@MembershipUserId
                                  ,@SubjectId
                                  ,@Issuer                                  
                                 )";

                var insertIdentityResult = await connection.ExecuteAsync(identityInsertquery, new
                {
                    MembershipUserId = insertMemberResult.Value,
                    SubjectId = subjectId,
                    Issuer = issuer
                }, transaction: transaction);

                if (insertIdentityResult != 1)
                {
                    _logger.LogError($"Error: Failed to create identity mapping for user: {insertMemberResult.Value}");
                    throw new DataException("Error: Failed to link user to identity mapping");
                }

                const string MemberRoleInsertquery =
                    @$" INSERT INTO   [dbo].[MembershipUsersInRoles]
                    
                                 ([UserIdentifier]
                                 ,[RoleIdentifier]
                                 )
                    VALUES
                                 (@MembershipUserId
                                  ,(
                                    SELECT [Id]
                                    FROM   [dbo].[MembershipRole]
                                    WHERE  [RoleName] = @DefaultRole
                                   )                               
                                 )";

                var MemberRoleInsertResult = await connection.ExecuteAsync(MemberRoleInsertquery, new
                {
                    MembershipUserId = insertMemberResult.Value,
                    DefaultRole = defaultRole,
                }, transaction: transaction);

                if (insertIdentityResult != 1)
                {
                    _logger.LogError($"Error: Failed to create the default role for user: {insertMemberResult.Value}");
                    throw new DataException("Error: Failed to create the default role");
                }

                await transaction.CommitAsync(cancellationToken);
                return insertMemberResult.Value;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }          
        }
    }
}

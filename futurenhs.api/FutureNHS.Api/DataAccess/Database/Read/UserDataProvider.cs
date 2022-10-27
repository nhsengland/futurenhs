using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Models.Identity.Response;
using FutureNHS.Api.Exceptions;
using FutureNHS.Api.Models.Member;
using Microsoft.Extensions.Options;
using FutureNHS.Api.DataAccess.Models.Identity;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class UserDataProvider : IUserDataProvider, IUserAdminDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<UserDataProvider> _logger;
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;

        public UserDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<UserDataProvider> logger,
            IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<(uint, IEnumerable<Member>)> GetMembersAsync(uint offset, uint limit, string sort, CancellationToken cancellationToken = default)
        {
            if (limit is < PaginationSettings.MinLimit or > PaginationSettings.MaxLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            const string query =
                @$" SELECT
                                [{nameof(Member.Id)}]                   = member.Id,
                                [{nameof(Member.Slug)}]                 = member.Slug, 
                                [{nameof(Member.Name)}]                 = member.FirstName + ' ' +  member.Surname, 
                                [{nameof(Member.DateJoinedUtc)}]        = member.CreatedAtUTC,
                                [{nameof(Member.LastLoginUtc)}]         = memberactivity.LastActivityDateUTC,
                                [{nameof(Member.Role)}]                 = memberRoles.RoleName

                    FROM        MembershipUser member 
					JOIN		MembershipUsersInRoles membersInRole 
					ON			membersInRole.UserIdentifier = member.Id
                    JOIN        MembershipRole memberRoles 
                    ON          membersInRole.RoleIdentifier = memberRoles.Id
                    JOIN        MembershipUserActivity memberactivity 
                    ON          memberactivity.MembershipUserId = member.Id
                    WHERE       member.IsDeleted = 0
                    ORDER BY    RoleName asc, member.FirstName asc

                    OFFSET      @Offset ROWS
                    FETCH NEXT  @Limit ROWS ONLY;

                    SELECT      COUNT(*) 

                    FROM        MembershipUser member;";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var reader = await dbConnection.QueryMultipleAsync(query, new
            {
                Offset = Convert.ToInt32(offset),
                Limit = Convert.ToInt32(limit)
            });

            var members = await reader.ReadAsync<Member>();

            var totalCount = Convert.ToUInt32(await reader.ReadFirstAsync<int>());

            return (totalCount, members);
        }

        public async Task<MemberDetails?> GetMemberAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT
                                [{nameof(MemberDetails.Id)}]                   = member.Id,
                                [{nameof(MemberDetails.Slug)}]                 = member.Slug, 
                                [{nameof(MemberDetails.FirstName)}]            = member.FirstName,
                                [{nameof(MemberDetails.LastName)}]             = member.Surname,
                                [{nameof(MemberDetails.Initials)}]             = member.Initials, 
                                [{nameof(MemberDetails.Email)}]                = member.Email, 
                                [{nameof(MemberDetails.Pronouns)}]             = member.Pronouns, 
                                [{nameof(MemberDetails.DateJoinedUtc)}]        = member.CreatedAtUTC,
                                [{nameof(MemberDetails.LastLoginUtc)}]         = memberactivity.LastActivityDateUTC,
                                [{nameof(MemberDetails.RowVersion)}]           = member.RowVersion 

                    FROM        MembershipUser member 
                    JOIN        MembershipUserActivity memberactivity 
                    ON          memberactivity.MembershipUserId = member.Id
                    WHERE       member.Id = @UserId
                    AND         member.IsDeleted = 0";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var member = await dbConnection.QuerySingleOrDefaultAsync<MemberDetails>(query, new
            {
                UserId = userId
            });

            return member;
        }


       

        public async Task<bool> IsMemberInvitedAsync(string emailAddress, CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT CASE WHEN EXISTS (
	                    SELECT *
	                    FROM GroupInvite
	                    WHERE  LOWER(EmailAddress) = LOWER(@EmailAddress)
	                    AND IsDeleted = 0
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT) END";

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            return await dbConnection.QuerySingleOrDefaultAsync<bool>(query, new
            {
                EmailAddress = emailAddress
            });
        }

        public async Task<MemberProfile> GetMemberProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            const string query =
                @$" SELECT
                                [{nameof(MemberProfile.Id)}]                = member.Id,
                                [{nameof(MemberProfile.FirstName)}]         = member.FirstName,
                                [{nameof(MemberProfile.LastName)}]          = member.Surname,
                                [{nameof(MemberProfile.Pronouns)}]          = member.Pronouns,
                                [{nameof(MemberProfile.ImageId)}]           = member.ImageId,
                                [{nameof(ImageData.Id)}]                    = image.Id,
                                [{nameof(ImageData.Height)}]                = image.Height,
                                [{nameof(ImageData.Width)}]                 = image.Width,
                                [{nameof(ImageData.FileName)}]              = image.FileName,
                                [{nameof(ImageData.MediaType)}]             = image.MediaType
				    
                    FROM        [MembershipUser] member
                    LEFT JOIN   Image image
                    ON          image.Id = member.ImageId
                    WHERE       member.[Id] = @Id
                    AND         member.IsDeleted = 0";


            var queryDefinition = new CommandDefinition(query, new
            {
                Id = userId,
            }, cancellationToken: cancellationToken);

            using var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken);

            var memberProfile = await dbConnection.QueryAsync<MemberProfile, Image, MemberProfile>(query,
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
                    Id = userId,
                }, splitOn: "id");

            return memberProfile.SingleOrDefault() ?? throw new NotFoundException("Member not found.");
        }
    }
}

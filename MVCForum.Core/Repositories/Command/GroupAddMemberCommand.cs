using Dapper;
using MvcForum.Core.Models.Enums;
using MvcForum.Core.Models.GroupAddMember;
using MvcForum.Core.Repositories.Command.Interfaces;
using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
using MvcForum.Core.Utilities;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Command
{
    public sealed class GroupAddMemberCommand : IGroupAddMemberCommand
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GroupAddMemberCommand(IDbConnectionFactory connectionFactory)
        {
            if (connectionFactory is null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public async Task<GroupAddMemberResponse> AddMemberToGroupAsync(MailAddress invitedUserMailAddress,
                                                                        string invitedUserRoleName,
                                                                        string addedByUsername,
                                                                        string invitedToGroupSlug,
                                                                        CancellationToken cancellationToken)
        {
            var groupUserId = GuidComb.GenerateComb();
            var dateTimeUtcNow = DateTime.UtcNow;

            const string query =
                @"
                    INSERT INTO
                        GroupUser (
                        [Id],
                        [Approved],
                        [Rejected],
                        [Locked],
                        [Banned],
                        [RequestToJoinDate],
                        [ApprovedToJoinDate],
                        [ApprovingMembershipUser_Id],
                        [MembershipRole_Id],
                        [MembershipUser_Id],
                        [Group_Id]
                    )
                    SELECT
	                    @groupUserId,
						1,
						0,
						0,
						0,
						@dateTimeUtcNow,
                        @dateTimeUtcNow,
						MembershipUserForInviter.Id,
						MembershipRoleForInvitee.Id,
	                    MembershipUserForInvitee.Id,	                  
	                    GroupForInvitee.Id
                    FROM
	                    MembershipUser MembershipUserForInvitee,
	                    MembershipUser MembershipUserForInviter,
	                    MembershipRole MembershipRoleForInvitee,
	                    [Group] GroupForInvitee
                    WHERE
	                    MembershipUserForInvitee.Email = @invitedUserMailAddressValue
                    AND
                        MembershipUserForInviter.UserName = @addedByUsername
                    AND
                        MembershipRoleForInvitee.RoleName = @invitedUserRoleName
                    AND
                        GroupForInvitee.Slug = @invitedToGroupSlug
                ";

                var commandDefinition = new CommandDefinition(query, new
                {
                    groupUserId,
                    dateTimeUtcNow,
                    invitedUserMailAddressValue = invitedUserMailAddress.Address,
                    addedByUsername,
                    invitedUserRoleName,
                    invitedToGroupSlug
                }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateWriteOnlyConnection())
            {
                var affectedRows = await dbConnection.ExecuteAsync(commandDefinition);

                if (affectedRows == 1)
                {
                    return new GroupAddMemberResponse(groupUserId, ResponseType.Success);
                }
            }

            return new GroupAddMemberResponse(null, ResponseType.Error);
        }
    }
}

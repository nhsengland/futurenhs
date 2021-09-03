using Dapper;
using MvcForum.Core.Models.Enums;
using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
using MvcForum.Core.Repositories.Repository.Interfaces;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Repository
{
    public sealed class GroupAddMemberRepository : IGroupAddMemberRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GroupAddMemberRepository(IDbConnectionFactory connectionFactory)
        {
            if (connectionFactory is null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public async Task<ResponseType> IsMemberMailAddressValidAsync(MailAddress invitedUserMailAddress,
                                                                      string invitedToGroupSlug, 
                                                                      CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT MemberExists = CAST(COUNT(1) AS BIT)
                    FROM   MembershipUser
                    WHERE  MembershipUser.Email = @invitedUserMailAddressValue;

                    SELECT MemberExistsInGroup = CAST(COUNT(1) AS BIT)
                    FROM   MembershipUser
                    JOIN GroupUser ON MembershipUser.Id = GroupUser.MembershipUser_Id
                    JOIN [Group] ON GroupUser.Group_Id = [Group].Id
                    WHERE  MembershipUser.Email = @invitedUserMailAddressValue 
	                  AND [Group].Slug = @invitedToGroupSlug;
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                invitedUserMailAddressValue = invitedUserMailAddress.Address,
                invitedToGroupSlug
            }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                var result = await dbConnection.QueryMultipleAsync(commandDefinition);
                var memberExists = result.ReadFirst<bool>();
                var memberExistsInGroup = result.ReadFirst<bool>();

                if (!memberExists)
                {
                    return ResponseType.DoesntExist;
                }

                if (memberExistsInGroup)
                {
                    return ResponseType.AlreadyExists;
                }

                return ResponseType.Success;
            }
        }
               
        public async Task<bool> IsCurrentMemberAdminAsync(string currentMemberUsername,
                                                          string invitedToGroupSlug,
                                                          CancellationToken cancellationToken)
        {
            const string query =
                @"
                    SELECT 
                        MembershipRole.RoleName
                    FROM   
                        MembershipUser
                    JOIN 
                        MembershipUsersInRoles ON MembershipUser.Id = MembershipUsersInRoles.UserIdentifier
					JOIN  
                        MembershipRole ON MembershipUsersInRoles.RoleIdentifier = MembershipRole.Id
                    WHERE  
                        MembershipUser.UserName = 'admin';


                    SELECT 
                        MembershipRole.RoleName
                    FROM
                        MembershipUser
                    JOIN 
                        GroupUser ON MembershipUser.Id = GroupUser.MembershipUser_Id
                    JOIN 
                        [Group] ON GroupUser.Group_Id = [Group].Id
                    JOIN 
                        MembershipRole ON GroupUser.MembershipRole_Id = MembershipRole.Id
                    WHERE  
                        MembershipUser.UserName = @currentMemberUsername 
	                AND 
                        [Group].Slug = @invitedToGroupSlug;
                ";


            var commandDefinition = new CommandDefinition(query, new
                {
                    currentMemberUsername,
                    invitedToGroupSlug
                }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                var result = await dbConnection.QueryMultipleAsync(query, new { invitedToGroupSlug, currentMemberUsername });

                var membershipRole = result.ReadFirstOrDefault<string>();
                var groupRole = result.ReadFirstOrDefault<string>();

                return membershipRole?.ToLower() == "admin" || groupRole?.ToLower() == "admin";
                
            }
        }
    }
}
using Dapper;
using FutureNHS.Api.Application.Application.HardCodedSettings;
using FutureNHS.Api.Configuration;
using FutureNHS.Api.DataAccess.Database.Providers.Interfaces;
using FutureNHS.Api.DataAccess.Database.Read.Interfaces;
using FutureNHS.Api.DataAccess.Models;
using FutureNHS.Api.DataAccess.Models.Group;
using FutureNHS.Api.DataAccess.Models.Registration;
using FutureNHS.Api.DataAccess.Models.User;
using FutureNHS.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.DataAccess.Database.Read
{
    public class RegistrationDataProvider : IRegistrationDataProvider
    {
        private readonly IAzureSqlDbConnectionFactory _connectionFactory;
        private readonly ILogger<RegistrationDataProvider> _logger;
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;

        public RegistrationDataProvider(IAzureSqlDbConnectionFactory connectionFactory, ILogger<RegistrationDataProvider> logger,
            IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<InviteDetails> GetRegistrationInviteAsync(Guid id, CancellationToken cancellationToken = default)
        {

            string query =
                @$"SELECT 
                    [{nameof(InviteDetails.Id)}]                        = platformInvite.Id,
                    [{nameof(GroupSummary.Id)}]                         = groups.Id,
                    [{nameof(GroupSummary.ThemeId)}]                    = groups.ThemeId,
                    [{nameof(GroupSummary.Slug)}]                       = groups.Slug,
                    [{nameof(GroupSummary.NameText)}]                   = groups.Name,
                    [{nameof(GroupSummary.StraplineText)}]              = groups.Subtitle,
                    [{nameof(GroupSummary.IsPublic)}]                   = groups.IsPublic,
                    [{nameof(GroupSummary.MemberCount)}]                = (SELECT COUNT(*) FROM GroupUser groupUser WHERE groupUser.Group_Id = groups.Id AND groupUser.Approved = 1 ), 
				    [{nameof(GroupSummary.DiscussionCount)}]            = (SELECT COUNT(*) FROM Discussion discussion WHERE discussion.Group_Id = groups.Id AND discussion.IsDeleted = 0),
                    [{nameof(ImageData.Id)}]		                    = image.Id,
                    [{nameof(ImageData.Height)}]	                    = image.Height,
                    [{nameof(ImageData.Width)}]		                    = image.Width,
                    [{nameof(ImageData.FileName)}]	                    = image.FileName,
                    [{nameof(ImageData.MediaType)}]	                    = image.MediaType,
                    [{nameof(UserNavProperty.Id)}]	                    = invitedBy.Id,  
                    [{nameof(UserNavProperty.Name)}]	                = invitedBy.FirstName + ' ' + invitedBy.Surname,
                    [{nameof(UserNavProperty.Slug)}]	                = invitedBy.Slug
				FROM [PlatformInvite] platformInvite
                LEFT JOIN [Group] groups ON groups.Id = platformInvite.GroupId
                LEFT JOIN MembershipUser invitedBy ON invitedBy.Id = platformInvite.CreatedBy
                LEFT JOIN Image image ON image.Id = groups.ImageId
                WHERE platformInvite.Id = @Id";

            using (var dbConnection = await _connectionFactory.GetReadOnlyConnectionAsync(cancellationToken))
            {
                
              var invite = await dbConnection.QueryAsync<InviteDetails, GroupSummary, ImageData, UserNavProperty, InviteDetails>(query,
                (invite, group, image, invitedBy) =>
              {
                  if (group is not null)
                  {
                      invite = @invite with
                      {
                          Group = new GroupSummary
                          {
                              Id = group.Id,
                              DiscussionCount = group.DiscussionCount,
                              IsPublic = group.IsPublic,
                              MemberCount = group.MemberCount,
                              NameText = group.NameText,
                              OwnerFirstName = group.OwnerFirstName,
                              OwnerId = group.OwnerId,
                              OwnerSurname = group.OwnerSurname,
                              Slug = group.Slug,
                              StraplineText = group.StraplineText,
                              ThemeId = group.ThemeId
                          }
                      };
                      if (image is not null)
                      {
                          invite = @invite with
                          {
                              Group = @group with {Image = new ImageData(image, _options)}
                          };
                      }
                  }
                  invite = @invite with { InvitedBy = invitedBy };


                  return invite;


              }, new
              {
                  Id = id
              }, splitOn: "id");

                return invite.FirstOrDefault();
            }          
        }
    }
}

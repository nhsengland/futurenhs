namespace MvcForum.Core.Repositories.Repository
{
    using Dapper;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using MvcForum.Core.Repositories.Repository.Interfaces;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<User> GetUser(string username, CancellationToken cancellationToken = default(CancellationToken))
        {
            User user;
            const string query =
                @"
                    SELECT 
                        Id AS Id,
                        Username As Username,
                        TRIM(ISNULL(FirstName, '') + ' ' + ISNULL(Surname, '')) AS FullNameText,
                        Initials AS Initials
                    FROM MembershipUser where Username = @Username
                ";

            var commandDefinition = new CommandDefinition(query, new
            {
                Username = username
            }, cancellationToken: cancellationToken);

            using (var dbConnection = _connectionFactory.CreateReadOnlyConnection())
            {
                user = await dbConnection.QuerySingleAsync<User>(commandDefinition);
            }

            return user;
        }

    }
}



using Dapper;
using MvcForum.Core.Models.SystemPages;
using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
using MvcForum.Core.Repositories.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Repository
{
    public class SystemPagesRepository : ISystemPagesRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SystemPagesRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<SystemPageViewModel>> GetSystemPages(CancellationToken cancellationToken)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();
            const string query =
                @"SELECT Id,Slug,Title
                FROM SystemPage            
                WHERE IsDeleted = 0";

            using (var multipleResults = await dbConnection.QueryMultipleAsync(query))
            {
                var results = await multipleResults.ReadAsync<SystemPageViewModel>();
                return results;
            }
        }

        public async Task<SystemPageViewModel> GetSystemPageById(Guid id, CancellationToken cancellationToken)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();
            const string query =
                @"SELECT Id, Slug, Title, Content 
                FROM SystemPage            
                WHERE Id = @Id AND IsDeleted = 0";

            var result = await dbConnection.QueryFirstOrDefaultAsync<SystemPageViewModel>(query, new { Id = id });

            return result;

        }

        public async Task<SystemPageViewModel> GetSystemPageBySlug(string slug, CancellationToken cancellationToken)
        {
            var dbConnection = _connectionFactory.CreateReadOnlyConnection();
            const string query =
                @"SELECT Id, Slug, Title, Content 
                FROM SystemPage            
                WHERE Slug = @Slug AND IsDeleted = 0";

            var result = await dbConnection.QueryFirstOrDefaultAsync<SystemPageViewModel>(query, new { Slug = slug });

            return result;
        }
    }
}

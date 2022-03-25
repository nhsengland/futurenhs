namespace MvcForum.Core.Repositories.Command
{
    using Dapper;
    using MvcForum.Core.Models.Groups;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class GroupCommand : IGroupCommand
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GroupCommand(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<bool> UpdateAsync(GroupWriteViewModel model, string slug, CancellationToken cancellationToken = default)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentNullException(nameof(slug));

            var sql = @"UPDATE [dbo].[Group] 
                        SET [Name] = @name, 
                            [Description] = @description,
                            [Image] = @image,
                            [PublicGroup] = @publicGroup,
                            [Introduction] = @introduction,
                            [AboutUs] = @aboutUs
                        WHERE [Slug] = @slug";

            var param = new
            {
                name = model.Name,
                description = model.Description,
                image = model.Image,
                publicGroup = model.PublicGroup,
                introduction = model.Introduction,
                aboutUs = model.AboutUs,
                slug = slug
            };

            using (var conn = _connectionFactory.CreateWriteOnlyConnection())
            {
                if (await conn.ExecuteAsync(sql, param) == 1)
                {
                    return true;
                }

                return false;
            }
        }
    }
}

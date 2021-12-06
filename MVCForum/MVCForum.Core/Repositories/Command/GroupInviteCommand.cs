namespace MvcForum.Core.Repositories.Command
{
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Models.Entities;
    using MvcForum.Core.Repositories.Command.Interfaces;
    using MvcForum.Core.Repositories.Models;
    using System;
    using System.Data.Entity.Core;
    using System.Threading;
    using System.Threading.Tasks;

    public class GroupInviteCommand : IGroupInviteCommand
    {
        private readonly IMvcForumContext _context;

        public GroupInviteCommand(IMvcForumContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateInviteAsync(GroupInviteViewModel model, CancellationToken cancellationToken)
        {
            var invite = new GroupInvite()
            {
                EmailAddress = model.EmailAddress,
                GroupId = model.GroupId,
                IsDeleted = model.IsDeleted
            };

            _context.GroupInvite.Add(invite);
            await _context.SaveChangesAsync(cancellationToken);
            return invite.Id;
        }

        public async Task<bool> DeleteInviteAsync(Guid inviteId, CancellationToken cancellationToken)
        {
            var result = await _context.GroupInvite.FindAsync(cancellationToken, new object[] { inviteId });
            if (result != null)
            {
                result.IsDeleted = true;

                int resultCount = await _context.SaveChangesAsync(cancellationToken);
                if (resultCount > 0)
                {
                    return true;
                }

                return false;
            }
            throw new ObjectNotFoundException();
        }
    }
}

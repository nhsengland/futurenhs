using MvcForum.Core.Repositories.Models;
using System;
using System.Threading.Tasks;

namespace MvcForum.Core.Repositories.Repository.Interfaces
{
    public interface IImageRepository
    {
        ImageViewModel Get(Guid id);
        Guid? GetGroupImageId(Guid groupId);
        Guid? GetMembershipUserImageId(Guid membershipUserId);
    }
}

namespace MvcForum.Core.Repositories.Command.Interfaces
{
    using MvcForum.Core.Repositories.Models;
    using System;

    public interface IImageCommand
    {
        Guid Create(ImageViewModel image);
        bool Update(ImageViewModel image);
        bool UpdateGroupImageId(Guid groupId, Guid imageId);
        bool UpdateMembershipUserImageId(Guid membershipUserId, Guid imageId);
    }
}

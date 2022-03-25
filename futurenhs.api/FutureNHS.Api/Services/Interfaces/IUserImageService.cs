using System.Drawing;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IUserImageService
    {
        TransformedImage TransformImageForAvatar(Stream image);
        string GetMimeType(byte[] imageAsByteArray);
        Task<Guid> CreateImageAsync(ImageDto image);
        Task DeleteImageAsync(Guid id);
    }
}
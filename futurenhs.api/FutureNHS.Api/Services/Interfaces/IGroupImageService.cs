using System.Drawing;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.Services.Interfaces
{
    public interface IGroupImageService
    {
        TransformedImage TransformImageForGroupHeader(Stream image);
        string GetMimeType(byte[] imageAsByteArray);
        Task<Guid> CreateImageAsync(ImageDto image);
        Task DeleteImageAsync(Guid id);
    }
}
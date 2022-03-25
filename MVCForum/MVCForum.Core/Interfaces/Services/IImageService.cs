using MvcForum.Core.Services;
using System.Drawing;

namespace MvcForum.Core.Interfaces.Services
{
    public interface IImageService
    {
        TransformedImage TransformImageForAvatar(Bitmap image);
        TransformedImage TransformImageForGroupHeader(Bitmap image);
        string GetMimeType(byte[] imageAsByteArray);
    }
}

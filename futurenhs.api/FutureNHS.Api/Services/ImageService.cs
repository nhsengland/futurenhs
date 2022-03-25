using System.Drawing;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;
using FutureNHS.Api.Services.Interfaces;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace FutureNHS.Api.Services
{
    public sealed class ImageService : IGroupImageService, IUserImageService
    {
        // Set the image compression to 50% quality
        const int ImageQuality = 50;

        private readonly IImageCommand _imageCommand;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IImageCommand imageCommand, ILogger<ImageService> logger)
        {
            _imageCommand = imageCommand;
            _logger = logger;
        }

        public TransformedImage TransformImageForAvatar(Stream image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));

            var imageSize = new Size(211, 211);

            return new TransformedImage
            {
                Image = CropResizeCompressFormatToWebpImage(image, imageSize, ImageQuality),
                Width = imageSize.Width,
                Height = imageSize.Height,
                MediaType = "image/webp"
            };
        }

        public TransformedImage TransformImageForGroupHeader(Stream image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));

            var imageSize = new Size(180, 180);

            return new TransformedImage
            {
                Image = CropResizeCompressFormatToWebpImage(image, imageSize, ImageQuality),
                Width = imageSize.Width,
                Height = imageSize.Height,
                MediaType = "image/webp"
            };
        }

        public string GetMimeType(byte[] imageAsByteArray)
        {
            if (imageAsByteArray is null) throw new ArgumentNullException(nameof(imageAsByteArray));
            if (imageAsByteArray.Length <= 0) throw new ArgumentNullException(nameof(imageAsByteArray));

            using var imageFactory = new ImageFactory();

            var image = imageFactory.Load(imageAsByteArray);
            return image.CurrentImageFormat.MimeType;
        }

        private byte[] CropResizeCompressFormatToWebpImage(Stream image, Size targetSize, int targetQuality)
        {
            if (targetSize.IsEmpty) throw new ArgumentOutOfRangeException(nameof(targetSize));
            if (targetQuality < 0) throw new ArgumentOutOfRangeException(nameof(targetQuality));

            using var ms = new MemoryStream();
            using var imageFactory = new ImageFactory(preserveExifData: false);

            var imageToTransform = imageFactory.Load(image);

            // Only works on Windows
            _ = imageToTransform
                .Crop(GetCropLayer(imageHeight: imageToTransform.Image.Height, imageWidth: imageToTransform.Image.Width, targetSize))
                .Resize(targetSize)
                .Quality(targetQuality)
                .Format(new WebPFormat())
                .Save(ms);

            return ms.ToArray();
        }

        private CropLayer GetCropLayer(double imageHeight, double imageWidth, Size aspectRatio)
        {
            if (imageHeight <= 0) throw new ArgumentOutOfRangeException(nameof(imageHeight));
            if (imageWidth <= 0) throw new ArgumentOutOfRangeException(nameof(imageWidth));
            if (aspectRatio.IsEmpty) throw new ArgumentOutOfRangeException(nameof(aspectRatio));

            double aspectRatioWidth = aspectRatio.Width;
            double aspectRatioHeight = aspectRatio.Height;

            var doesImageWidthNeedReducing = imageWidth / imageHeight > aspectRatioWidth / aspectRatioHeight;
            if (doesImageWidthNeedReducing)
            {
                var extraWidth = imageWidth - (imageHeight * (aspectRatioWidth / aspectRatioHeight));
                return new CropLayer((int)extraWidth / 2, 0, (int)(imageWidth - extraWidth), (int)imageHeight, CropMode.Pixels);
            }

            var doesImageHeightNeedReducing = imageHeight / imageWidth > aspectRatioWidth / aspectRatioHeight;
            if (doesImageHeightNeedReducing)
            {
                var extraHeight = imageHeight - (imageWidth * (aspectRatioHeight / aspectRatioWidth));
                return new CropLayer(0, (int)extraHeight / 2, (int)imageWidth, (int)(imageHeight - extraHeight), CropMode.Pixels);
            }

            return new CropLayer(0, 0, (int)imageWidth, (int)imageHeight, CropMode.Pixels);
        }

        public async Task<Guid> CreateImageAsync(ImageDto image)
        {
           return await _imageCommand.CreateImageAsync(image);
        }

        public async Task DeleteImageAsync(Guid id)
        {
            await _imageCommand.ForceDeleteImageAsync(id);
        }
    }

    public sealed class TransformedImage
    {
        public byte[] Image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string MediaType { get; set; }
    }
}


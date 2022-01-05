using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using MvcForum.Core.Interfaces.Services;
using System;
using System.Drawing;
using System.IO;

namespace MvcForum.Core.Services
{
    public sealed class ImageService : IImageService
    {
        const int ImageQuality = 50;
        public TransformedImage TransformImageForAvatar(Bitmap image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));

            var imageSize = new Size(211, 211);

            return new TransformedImage
            {
                Bytes = CropResizeCompressFormatToWebpImage(image, imageSize, ImageQuality),
                Width = imageSize.Width,
                Height = imageSize.Height,
                MediaType = "image/webp"
            };
        }

        public TransformedImage TransformImageForGroupHeader(Bitmap image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));

            var imageSize = new Size(159, 159);

            return new TransformedImage
            {
                Bytes = CropResizeCompressFormatToWebpImage(image, imageSize, ImageQuality),
                Width = imageSize.Width,
                Height = imageSize.Height,
                MediaType = "image/webp"
            };
        }

        public string GetMimeType(byte[] imageAsByteArray)
        {
            if (imageAsByteArray is null) throw new ArgumentNullException(nameof(imageAsByteArray));
            if (imageAsByteArray.Length <= 0) throw new ArgumentNullException(nameof(imageAsByteArray));

            using (var imageFactory = new ImageFactory())
            {
                var image = imageFactory.Load(imageAsByteArray);
                return image.CurrentImageFormat.MimeType;
            }
        }

        private byte[] CropResizeCompressFormatToWebpImage(Bitmap image, Size targetSize, int targetQuality)
        {
            if (targetSize.IsEmpty) throw new ArgumentOutOfRangeException(nameof(targetSize));
            if (targetQuality < 0) throw new ArgumentOutOfRangeException(nameof(targetQuality));

            using (var ms = new MemoryStream())
            using (var imageFactory = new ImageFactory(preserveExifData: false))
            {
                var imageToTransform = imageFactory.Load(image);

                imageToTransform
                    .Crop(GetCropLayer(imageToTransform.Image.Height, imageToTransform.Image.Width, targetSize))
                    .Resize(targetSize)
                    .Quality(targetQuality)
                    .Format(new WebPFormat())
                    .Save(ms);

                return ms.ToArray();
            }
        }

        private CropLayer GetCropLayer(double imageHeight, double imageWidth, Size aspectRatio)
        {
            if (imageHeight <= 0) throw new ArgumentOutOfRangeException(nameof(imageHeight));
            if (imageWidth <= 0) throw new ArgumentOutOfRangeException(nameof(imageWidth));
            if (aspectRatio.IsEmpty) throw new ArgumentOutOfRangeException(nameof(aspectRatio));

            double aspectRatioWidth = aspectRatio.Width;
            double aspectRatioHeight = aspectRatio.Height;

            bool doesImageWidthNeedReducing = imageWidth / imageHeight > aspectRatioWidth / aspectRatioHeight;
            if (doesImageWidthNeedReducing)
            {
                double extraWidth = imageWidth - (imageHeight * (aspectRatioWidth / aspectRatioHeight));
                return new CropLayer((int)extraWidth / 2, 0, (int)(imageWidth - extraWidth), (int)imageHeight, CropMode.Pixels);
            }

            bool doesImageHeightNeedReducing = imageHeight / imageWidth > aspectRatioWidth / aspectRatioHeight;
            if (doesImageHeightNeedReducing)
            {
                double extraHeight = imageHeight - (imageWidth * (aspectRatioHeight / aspectRatioWidth));
                return new CropLayer(0, (int)extraHeight / 2, (int)imageWidth, (int)(imageHeight - extraHeight), CropMode.Pixels);
            }

            return new CropLayer(0, 0, (int)imageWidth, (int)imageHeight, CropMode.Pixels);
        }
    }

    public sealed class TransformedImage
    {
        public byte[] Bytes { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string MediaType { get; set; }
    }
}

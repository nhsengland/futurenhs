using ImageProcessor;
using MvcForum.Core.Services;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MvcForum.Core.Tests.Services.Image
{
    public sealed class ImageServiceTests
    {
        private ImageService _imageService;

        [SetUp]
        public void SetUp()
        {
            _imageService = new ImageService();
        }

        [DatapointSource]
        public Size[] imageSizeValues = new Size[] 
        { 
            new Size(500, 1000),
            new Size(1000, 500),
            new Size(20, 50),
            new Size(50, 20),
            new Size(1000, 20),
            new Size(20, 1000),
            new Size(500, 500),
            new Size(50, 50),
        };

        [Theory]
        public void TransformImageForGroupHeader_VariousImageSizes_CropsAroundCenter(Size size)
        {
            using (var image = GenerateImageWithCenterDot(size))
            using (var transformedImageStream = new MemoryStream(_imageService.TransformImageForGroupHeader(image).Bytes))
            using (var imageFactory = new ImageFactory())
            {                
                var imageToTest = imageFactory.Load(transformedImageStream);
                var imageToTestBitmap = new Bitmap(imageToTest.Image);
                bool isDotInCenter = imageToTestBitmap.GetPixel(imageToTestBitmap.Width / 2, imageToTestBitmap.Height / 2) != Color.White;
                Assert.IsTrue(isDotInCenter);
            }
        }

        [Theory]
        public void TransformImageForGroupHeader_VariousImageSizes_ReturnsCorrectSize(Size size)
        {
            using (var image = GenerateImageWithCenterDot(size))
            using (var transformedImageStream = new MemoryStream(_imageService.TransformImageForGroupHeader(image).Bytes))
            using (var imageFactory = new ImageFactory())
            {
                var imageToTest = imageFactory.Load(transformedImageStream);
                var imageToTestBitmap = new Bitmap(imageToTest.Image);

                Assert.IsTrue(imageToTestBitmap.Width == 180);
                Assert.IsTrue(imageToTestBitmap.Height == 180);
            }
        }

        [Test]
        public void TransformImageForGroupHeader_LargerThanTargetImageSizes_CompressesImage()
        {
            var largeSize = new Size(2000, 4000);
            using (var originalImage = GenerateImageWithCenterDot(largeSize))
            using (var originalImageMemoryStream = new MemoryStream())
            using (var transformedImageMemoryStream = new MemoryStream())
            {
                originalImage.Save(originalImageMemoryStream, ImageFormat.Png);
                long originalImageSize = originalImageMemoryStream.Length;

                var transformedImage = _imageService.TransformImageForGroupHeader(originalImage);
    
                Assert.IsTrue(originalImageSize > transformedImage.Bytes.Length);
            }
        }

        [Test]
        public void TransformImageForGroupHeader_NullParameter_Exception()
        {
            Bitmap bitmap = null;
            var response = Assert.Throws<ArgumentNullException>( () => _imageService.TransformImageForGroupHeader(bitmap));
            Assert.AreEqual("image", response.ParamName);
        }

        [Theory]
        public void TransformImageForAvatar_VariousImageSizes_CropsAroundCenter(Size size)
        {
            using (var image = GenerateImageWithCenterDot(size))
            using (var transformedImageStream = new MemoryStream(_imageService.TransformImageForAvatar(image).Bytes))
            using (var imageFactory = new ImageFactory())
            {
                var imageToTest = imageFactory.Load(transformedImageStream);
                var imageToTestBitmap = new Bitmap(imageToTest.Image);

                bool isDotInCenter = imageToTestBitmap.GetPixel(imageToTestBitmap.Width / 2, imageToTestBitmap.Height / 2) != Color.White;
                Assert.IsTrue(isDotInCenter);
            }
        }

        [Theory]
        public void TransformImageForAvatar_VariousImageSizes_ReturnsCorrectSize(Size size)
        {
            using (var image = GenerateImageWithCenterDot(size))
            using (var transformedImageStream = new MemoryStream(_imageService.TransformImageForAvatar(image).Bytes))
            using (var imageFactory = new ImageFactory())
            {
                var imageToTest = imageFactory.Load(transformedImageStream);
                var imageToTestBitmap = new Bitmap(imageToTest.Image);

                Assert.IsTrue(imageToTestBitmap.Width == 211);
                Assert.IsTrue(imageToTestBitmap.Height == 211);
            }
        }

        [Test]
        public void TransformImageForAvatar_LargerThanTargetImageSizes_CompressesImage()
        {
            var largeSize = new Size(100, 150);
            using (var originalImage = GenerateImageWithCenterDot(largeSize))
            using (var originalImageMemoryStream = new MemoryStream())
            using (var transformedImageMemoryStream = new MemoryStream())
            {
                originalImage.Save(originalImageMemoryStream, ImageFormat.Png);
                long originalImageSize = originalImageMemoryStream.Length;

                var transformedImage = _imageService.TransformImageForGroupHeader(originalImage);

                Assert.IsTrue(originalImageSize > transformedImage.Bytes.Length);
            }
        }

        [Test]
        public void TransformImageForAvatar_NullParameter_Exception()
        {
            Bitmap bitmap = null;
            var response = Assert.Throws<ArgumentNullException>(() => _imageService.TransformImageForAvatar(bitmap));
            Assert.AreEqual("image", response.ParamName);
        }

        [Theory]
        public void GetMimeType_WebpImage_CorrectMimeType()
        {
            var imageSize = new Size(300, 300);
            using (var image = GenerateImageWithCenterDot(imageSize))
            {
                var transformedImageBytes = _imageService.TransformImageForGroupHeader(image).Bytes;
                var transformedImageMimeType = _imageService.GetMimeType(transformedImageBytes);
                Assert.IsTrue(transformedImageMimeType == "image/webp");
            }
        }

        private Bitmap GenerateImageWithCenterDot(Size size)
        {
            var bitmap = new Bitmap(size.Width, size.Height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);

                bitmap.SetPixel(bitmap.Width / 2, bitmap.Height / 2, Color.Black);
                return bitmap;
            }
        }
    }
}

namespace MvcForum.Core.ExtensionMethods
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Models.General;
    using Providers.Storage;

    public static class ImageExtensions
    {
        /// <summary>
        ///     Convert an image to base64 string
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string ImageToBase64(this Image image)
        {
            using (var m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                var imageBytes = m.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        /// <summary>
        ///     Converts a BAse 64 string back into an image
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static Image Base64ToImage(this string base64String)
        {
            var imageBytes = Convert.FromBase64String(base64String);
            Image image;
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                image = Image.FromStream(ms, true);
            }
            return image;
        }


        /// <summary>
        /// Turns a posted image to a C# image and rotates as needed
        /// </summary>
        /// <param name="httpPostedFile"></param>
        /// <returns></returns>
        public static Image ToImage(this HttpPostedFileBase httpPostedFile)
        {
            // Rotate image if wrong want around
            var sourceimage = Image.FromStream(httpPostedFile.InputStream, true, true);

            if (sourceimage.PropertyIdList.Contains(0x0112))
            {
                int rotationValue = sourceimage.GetPropertyItem(0x0112).Value[0];
                switch (rotationValue)
                {
                    case 1: // landscape, do nothing
                        break;

                    case 8: // rotated 90 right
                            // de-rotate:
                        sourceimage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;

                    case 3: // bottoms up
                        sourceimage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;

                    case 6: // rotated 90 left
                        sourceimage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                }
            }

            return sourceimage;
        }

        /// <summary>
        /// Uploads an Image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fileName"></param>
        /// <param name="uploadFolderPath"></param>
        /// <returns></returns>
        [Obsolete]
        public static UploadFileResult Upload(this Image image, string uploadFolderPath, string fileName)
        {
            var extension = Path.GetExtension(fileName);
            fileName = Guid.NewGuid().ToString();
            var upResult = new UploadFileResult { UploadSuccessful = true };

            try
            {
                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }

                // If no file name make one
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = $"{fileName.ToLower()}.jpg";
                }
                else
                {
                    fileName = $"{fileName.ToLower()}{extension}";
                }

                using (var stream = new MemoryStream())
                {
                    // Save the image as a Jpeg only
                    var bmp = new Bitmap(image);
                    bmp.Save(stream, ImageFormat.Jpeg);
                    stream.Position = 0;

                    var file = new MemoryFile(stream, "image/jpeg", fileName);

                    // Get the storage provider and save file
                    upResult.UploadedFileName = fileName;
                    upResult.UploadedFileUrl = StorageProvider.Current.SaveAs(uploadFolderPath, fileName, file.InputStream);
                }
            }
            catch (Exception ex)
            {
                upResult.UploadSuccessful = false;
                upResult.ErrorMessage = ex.Message;
            }

            return upResult;
        }

        /// <summary>
        /// Removes metadata from image
        /// for full list of property item ids
        /// https://docs.microsoft.com/en-us/dotnet/api/system.drawing.imaging.propertyitem.id?redirectedfrom=MSDN&view=net-5.0#System_Drawing_Imaging_PropertyItem_Id
        /// </summary>
        /// <param name="img"></param>
        /// <returns>System.Drawing.Image with all metadata removed</returns>
        public static Image StripMetaData(this Image img)
        {
            foreach (PropertyItem item in img.PropertyItems) {
                PropertyItem modItem = item;
                modItem.Value = new byte[] { 0 };
                img.SetPropertyItem(modItem);
            }
            return img;
        }

        /// <summary>
        /// converts image into MemoryStream
        /// </summary>
        /// <param name="img"></param>
        /// <returns>Steam</returns>
        public static Stream ToMemoryStream(this Image img)
        {
            MemoryStream myMemoryStream = new MemoryStream();
            img.Save(myMemoryStream, img.RawFormat);
            return myMemoryStream;
        }
    }
}
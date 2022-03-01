using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace FutureNHS.Api.Helpers
{
    public static class FileHelper
    {
        // If you require a check on specific characters in the IsValidFileExtensionAndSignature
        // method, supply the characters in the _allowedChars field.
        private static readonly byte[] AllowedChars = { };
        // For more file signatures, see the File Signatures Database (https://www.filesignatures.net/)
        // and the official specifications for the file types you wish to add.
        private static readonly Dictionary<string, List<byte[]>> FileSignature = new Dictionary<string, List<byte[]>>
{
    { ".accdb", new List<byte[]> { new byte[] { 0x00, 0x01, 0x00, 0x00, 0x53, 0x74, 0x61, 0x6E, 0x64, 0x61, 0x72, 0x64, 0x20, 0x41, 0x43, 0x45, 0x20, 0x44, 0x42 } } },
    { ".aiff", new List<byte[]> { new byte[] { 0x46, 0x4F, 0x52, 0x4D, 0x00 } } },
    { ".asf", new List<byte[]> { new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11 } } },
    { ".asx", new List<byte[]> { new byte[] { 0x3C } } },
    { ".au", new List<byte[]> { new byte[] { 0x64, 0x6E, 0x73, 0x2E }, new byte[] { 0x2E, 0x73, 0x6E, 0x64 } } },
    { ".avi", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } } },
    { ".bin", new List<byte[]> { new byte[] { 0x42, 0x4C, 0x49, 0x32, 0x32, 0x33, 0x51 } } },
    { ".bmp", new List<byte[]> { new byte[] { 0x42, 0x4D } } },
    { ".cab", new List<byte[]> { new byte[] { 0x49, 0x53, 0x63, 0x28 }, new byte[] { 0x4D, 0x53, 0x43, 0x46 } } },
    { ".cat", new List<byte[]> { new byte[] { 0x30 } } },
    { ".chm", new List<byte[]> { new byte[] { 0x49, 0x54, 0x53, 0x46 } } },
    { ".class", new List<byte[]> { new byte[] { 0xCA, 0xFE, 0xBA, 0xBE } } },
    { ".cmx", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } } },
    { ".cod", new List<byte[]> { new byte[] { 0x4E, 0x61, 0x6D, 0x65, 0x3A, 0x20 } } },
    { ".csh", new List<byte[]> { new byte[] { 0x63, 0x75, 0x73, 0x68, 0x00, 0x00, 0x00, 0x02 } } },
    { ".cur", new List<byte[]> { new byte[] { 0x00, 0x00, 0x02, 0x00 } } },
    { ".dib", new List<byte[]> { new byte[] { 0x42, 0x4D } } },
    { ".dll", new List<byte[]> { new byte[] { 0x4D, 0x5A } } },
    { ".doc", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }, new byte[] { 0x0D, 0x44, 0x4F, 0x43 }, new byte[] { 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00 }, new byte[] { 0xDB, 0xA5, 0x2D, 0x00 }, new byte[] { 0xEC, 0xA5, 0xC1, 0x00 } } },
    { ".docx", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 }, new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } } },
    { ".dot", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
    { ".dsp", new List<byte[]> { new byte[] { 0x23, 0x20, 0x4D, 0x69, 0x63, 0x72, 0x6F, 0x73 } } },
    { ".dtd", new List<byte[]> { new byte[] { 0x07, 0x64, 0x74, 0x32, 0x64, 0x64, 0x74, 0x64 } } },
    { ".eml", new List<byte[]> { new byte[] { 0x58, 0x2D }, new byte[] { 0x52, 0x65, 0x74, 0x75, 0x72, 0x6E, 0x2D, 0x50 }, new byte[] { 0x46, 0x72, 0x6F, 0x6D } } },
    { ".eps", new List<byte[]> { new byte[] { 0xC5, 0xD0, 0xD3, 0xC6 }, new byte[] { 0x25, 0x21, 0x50, 0x53, 0x2D, 0x41, 0x64, 0x6F } } },
    { ".exe", new List<byte[]> { new byte[] { 0x4D, 0x5A } } },
    { ".fdf", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
    { ".flv", new List<byte[]> { new byte[] { 0x46, 0x4C, 0x56 } } },
    { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
    { ".gz", new List<byte[]> { new byte[] { 0x1F, 0x8B, 0x08 } } },
    { ".hlp", new List<byte[]> { new byte[] { 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }, new byte[] { 0x3F, 0x5F, 0x03, 0x00 }, new byte[] { 0x4C, 0x4E, 0x02, 0x00 } } },
    { ".hqx", new List<byte[]> { new byte[] { 0x28, 0x54, 0x68, 0x69, 0x73, 0x20, 0x66, 0x69 } } },
    { ".ico", new List<byte[]> { new byte[] { 0x00, 0x00, 0x01, 0x00 } } },
    { ".jar", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 }, new byte[] { 0x5F, 0x27, 0xA8, 0x89 }, new byte[] { 0x4A, 0x41, 0x52, 0x43, 0x53, 0x00 }, new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x08, 0x00 } } },
    { ".jfif", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } } },
    { ".jpe", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } } },
    { ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 } } },
    { ".jpg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 } } },
    { ".lit", new List<byte[]> { new byte[] { 0x49, 0x54, 0x4F, 0x4C, 0x49, 0x54, 0x4C, 0x53 } } },
    { ".lzh", new List<byte[]> { new byte[] { 0x2D, 0x6C, 0x68 } } },
    { ".manifest", new List<byte[]> { new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D } } },
    { ".mdb", new List<byte[]> { new byte[] { 0x00, 0x01, 0x00, 0x00, 0x53, 0x74, 0x61, 0x6E, 0x64, 0x61, 0x72, 0x64, 0x20, 0x4A, 0x65, 0x74, 0x20, 0x44, 0x42 } } },
    { ".mid", new List<byte[]> { new byte[] { 0x4D, 0x54, 0x68, 0x64 } } },
    { ".midi", new List<byte[]> { new byte[] { 0x4D, 0x54, 0x68, 0x64 } } },
    { ".mmf", new List<byte[]> { new byte[] { 0x4D, 0x4D, 0x4D, 0x44, 0x00, 0x00 } } },
    { ".mny", new List<byte[]> { new byte[] { 0x00, 0x01, 0x00, 0x00, 0x4D, 0x53, 0x49, 0x53, 0x41, 0x4D, 0x20, 0x44, 0x61, 0x74, 0x61, 0x62, 0x61, 0x73, 0x65 } } },
    { ".mov", new List<byte[]> { new byte[] { 0x6D, 0x6F, 0x6F, 0x76 }, new byte[] { 0x66, 0x72, 0x65, 0x65 }, new byte[] { 0x6D, 0x64, 0x61, 0x74 }, new byte[] { 0x77, 0x69, 0x64, 0x65 }, new byte[] { 0x70, 0x6E, 0x6F, 0x74 }, new byte[] { 0x73, 0x6B, 0x69, 0x70 } } },
    { ".mp3", new List<byte[]> { new byte[] { 0x49, 0x44, 0x33 } } },
    { ".mpg", new List<byte[]> { new byte[] { 0x00, 0x00, 0x01, 0xBA }, new byte[] { 0x00, 0x00, 0x01, 0xB3 } } },
    { ".msi", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }, new byte[] { 0x23, 0x20 } } },
    { ".ocx", new List<byte[]> { new byte[] { 0x4D, 0x5A } } },
    { ".one", new List<byte[]> { new byte[] { 0xE4, 0x52, 0x5C, 0x7B, 0x8C, 0xD8, 0xA7, 0x4D } } },
    { ".p10", new List<byte[]> { new byte[] { 0x64, 0x00, 0x00, 0x00 } } },
    { ".pcx", new List<byte[]> { new byte[] { 0x0A, 0x02, 0x01, 0x01 }, new byte[] { 0x0A, 0x03, 0x01, 0x01 }, new byte[] { 0x0A, 0x05, 0x01, 0x01 } } },
    { ".pdf", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
    { ".pgm", new List<byte[]> { new byte[] { 0x50, 0x35, 0x0A } } },
    { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
    { ".pps", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
    { ".ppt", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }, new byte[] { 0x00, 0x6E, 0x1E, 0xF0 }, new byte[] { 0x0F, 0x00, 0xE8, 0x03 }, new byte[] { 0xA0, 0x46, 0x1D, 0xF0 }, new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x0E, 0x00, 0x00, 0x00 }, new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00 }, new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x43, 0x00, 0x00, 0x00 } } },
    { ".pptx", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 }, new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } } },
    { ".psd", new List<byte[]> { new byte[] { 0x38, 0x42, 0x50, 0x53 } } },
    { ".psp", new List<byte[]> { new byte[] { 0x7E, 0x42, 0x4B, 0x00 } } },
    { ".pub", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
    { ".qxd", new List<byte[]> { new byte[] { 0x00, 0x00, 0x49, 0x49, 0x58, 0x50, 0x52 }, new byte[] { 0x00, 0x00, 0x4D, 0x4D, 0x58, 0x50, 0x52 } } },
    { ".ra", new List<byte[]> { new byte[] { 0x2E, 0x52, 0x4D, 0x46, 0x00, 0x00, 0x00, 0x12 }, new byte[] { 0x2E, 0x72, 0x61, 0xFD, 0x00 } } },
    { ".ram", new List<byte[]> { new byte[] { 0x72, 0x74, 0x73, 0x70, 0x3A, 0x2F, 0x2F } } },
    { ".rar", new List<byte[]> { new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00 } } },
    { ".rgb", new List<byte[]> { new byte[] { 0x01, 0xDA, 0x01, 0x01, 0x00, 0x03 } } },
    { ".rm", new List<byte[]> { new byte[] { 0x2E, 0x52, 0x4D, 0x46 } } },
    { ".rmi", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } } },
    { ".rpm", new List<byte[]> { new byte[] { 0xED, 0xAB, 0xEE, 0xDB } } },
    { ".rtf", new List<byte[]> { new byte[] { 0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31 } } },
    { ".sit", new List<byte[]> { new byte[] { 0x53, 0x49, 0x54, 0x21, 0x00 }, new byte[] { 0x53, 0x74, 0x75, 0x66, 0x66, 0x49, 0x74, 0x20 } } },
    { ".snp", new List<byte[]> { new byte[] { 0x4D, 0x53, 0x43, 0x46 } } },
    { ".spl", new List<byte[]> { new byte[] { 0x00, 0x00, 0x01, 0x00 } } },
    { ".swf", new List<byte[]> { new byte[] { 0x43, 0x57, 0x53 }, new byte[] { 0x46, 0x57, 0x53 } } },
    { ".tar", new List<byte[]> { new byte[] { 0x75, 0x73, 0x74, 0x61, 0x72 } } },
    { ".tif", new List<byte[]> { new byte[] { 0x49, 0x20, 0x49 }, new byte[] { 0x49, 0x49, 0x2A, 0x00 }, new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, new byte[] { 0x4D, 0x4D, 0x00, 0x2B } } },
    { ".tiff", new List<byte[]> { new byte[] { 0x49, 0x20, 0x49 }, new byte[] { 0x49, 0x49, 0x2A, 0x00 }, new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, new byte[] { 0x4D, 0x4D, 0x00, 0x2B } } },
    { ".vcf", new List<byte[]> { new byte[] { 0x42, 0x45, 0x47, 0x49, 0x4E, 0x3A, 0x56, 0x43 } } },
    { ".vsd", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
    { ".wav", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } } },
    { ".wks", new List<byte[]> { new byte[] { 0x0E, 0x57, 0x4B, 0x53 }, new byte[] { 0xFF, 0x00, 0x02, 0x00, 0x04, 0x04, 0x05, 0x54 } } },
    { ".wma", new List<byte[]> { new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11 } } },
    { ".wmf", new List<byte[]> { new byte[] { 0xD7, 0xCD, 0xC6, 0x9A } } },
    { ".wmv", new List<byte[]> { new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11 } } },
    { ".wmz", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
    { ".wps", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
    { ".wri", new List<byte[]> { new byte[] { 0x31, 0xBE }, new byte[] { 0x32, 0xBE }, new byte[] { 0xBE, 0x00, 0x00, 0x00, 0xAB } } },
    { ".xdr", new List<byte[]> { new byte[] { 0x3C } } },
    { ".xla", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
    { ".xls", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }, new byte[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 }, new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x10 }, new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x1F }, new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x22 }, new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x23 }, new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x28 }, new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x29 } } },
    { ".xlsx", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 }, new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } } },
    { ".xml", new List<byte[]> { new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22, 0x3F, 0x3E } } },
    { ".xps", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
    { ".zip", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 }, new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45 }, new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 }, new byte[] { 0x50, 0x4B, 0x05, 0x06 }, new byte[] { 0x50, 0x4B, 0x07, 0x08 }, new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 }, new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x01, 0x00 } } },
};

        // **WARNING!**
        // In the following file processing methods, the file's content isn't scanned.
        // In most production scenarios, an anti-virus/anti-malware scanner API is
        // used on the file before making the file available to users or other
        // systems. For more information, see the topic that accompanies this sample
        // app.

        public static async Task<byte[]> ProcessFormFile<T>(IFormFile formFile,
            ModelStateDictionary modelState, string[] permittedExtensions,
            long sizeLimit)
        {
            var fieldDisplayName = string.Empty;

            // Use reflection to obtain the display name for the model
            // property associated with this IFormFile. If a display
            // name isn't found, error messages simply won't show
            // a display name.
            MemberInfo? property =
                typeof(T).GetProperty(
                    formFile.Name.Substring(formFile.Name.IndexOf(".",
                    StringComparison.Ordinal) + 1));

            if (property != null)
            {
                if (property.GetCustomAttribute(typeof(DisplayAttribute)) is
                    DisplayAttribute displayAttribute)
                {
                    fieldDisplayName = $"{displayAttribute.Name} ";
                }
            }

            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                formFile.FileName);

            // Check the file length. This check doesn't catch files that only have 
            // a BOM as their content.
            if (formFile.Length == 0)
            {
                modelState.AddModelError(formFile.Name,
                    $"{fieldDisplayName}({trustedFileNameForDisplay}) is empty.");

                return Array.Empty<byte>();
            }

            if (formFile.Length > sizeLimit)
            {
                var megabyteSizeLimit = sizeLimit / 1048576;
                modelState.AddModelError(formFile.Name,
                    $"{fieldDisplayName}({trustedFileNameForDisplay}) exceeds " +
                    $"{megabyteSizeLimit:N1} MB.");

                return Array.Empty<byte>();
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);

                    // Check the content length in case the file's only
                    // content was a BOM and the content is actually
                    // empty after removing the BOM.
                    if (memoryStream.Length == 0)
                    {
                        modelState.AddModelError(formFile.Name,
                            $"{fieldDisplayName}({trustedFileNameForDisplay}) is empty.");
                    }

                    if (!IsValidFileExtensionAndSignature(
                            formFile.FileName, memoryStream, permittedExtensions))
                    {
                        modelState.AddModelError(formFile.Name,
                            $"{fieldDisplayName}({trustedFileNameForDisplay}) file " +
                            "type isn't permitted or the file's signature " +
                            "doesn't match the file's extension.");
                    }
                    else
                    {
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                modelState.AddModelError(formFile.Name,
                    $"{fieldDisplayName}({trustedFileNameForDisplay}) upload failed. " +
                    $"Please contact the Help Desk for support. Error: {ex.HResult}");
                // Log the exception
            }

            return Array.Empty<byte>();
        }

        public static async Task<byte[]> ProcessStreamedFile(
            MultipartSection section, ContentDispositionHeaderValue contentDisposition,
            ModelStateDictionary modelState, string[] permittedExtensions, long sizeLimit)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await section.Body.CopyToAsync(memoryStream);

                    // Check if the file is empty or exceeds the size limit.
                    if (memoryStream.Length == 0)
                    {
                        modelState.AddModelError("File", "The file is empty.");
                    }
                    else if (memoryStream.Length > sizeLimit)
                    {
                        var megabyteSizeLimit = sizeLimit / 1048576;
                        modelState.AddModelError("File",
                        $"The file exceeds {megabyteSizeLimit:N1} MB.");
                    }
                    else if (!IsValidFileExtensionAndSignature(
                        contentDisposition.FileName.Value, memoryStream,
                        permittedExtensions))
                    {
                        modelState.AddModelError("File",
                            "The file type isn't permitted or the file's " +
                            "signature doesn't match the file's extension.");
                    }
                    else
                    {
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                modelState.AddModelError("File",
                    "The upload failed. Please contact the Help Desk " +
                    $" for support. Error: {ex.HResult}");
                // Log the exception
            }

            return Array.Empty<byte>();
        }

        public static bool IsValidFileExtensionAndSignature(string fileName, Stream data, string[] permittedExtensions)
        {
            if (string.IsNullOrEmpty(fileName) || data.Length == 0)
            {
                return false;
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            data.Position = 0;

            using (var reader = new BinaryReader(data))
            {
                if (ext.Equals(".txt") || ext.Equals(".csv") || ext.Equals(".prn"))
                {
                    if (AllowedChars.Length == 0)
                    {
                        // Limits characters to ASCII encoding.
                        for (var i = 0; i < data.Length; i++)
                        {
                            if (reader.ReadByte() > sbyte.MaxValue)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        // Limits characters to ASCII encoding and
                        // values of the _allowedChars array.
                        for (var i = 0; i < data.Length; i++)
                        {
                            var b = reader.ReadByte();
                            if (b > sbyte.MaxValue ||
                                !AllowedChars.Contains(b))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }

                // Uncomment the following code block if you must permit
                // files whose signature isn't provided in the _fileSignature
                // dictionary. We recommend that you add file signatures
                // for files (when possible) for all file types you intend
                // to allow on the system and perform the file signature
                // check.
                /*
                if (!_fileSignature.ContainsKey(ext))
                {
                    return true;
                }
                */

                // File signature check
                // --------------------
                // With the file signatures provided in the _fileSignature
                // dictionary, the following code tests the input content's
                // file signature.
                var signatures = FileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                return signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }
    }
}
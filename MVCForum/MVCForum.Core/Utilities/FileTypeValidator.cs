namespace MvcForum.Core.Utilities
{
    using MimeDetective;
    using MvcForum.Core.Interfaces.Helpers;
    using System;
    using System.IO;
    using System.Linq;

    public sealed class FileTypeValidator : IValidateFileType
    {
        private readonly ContentInspector _contentInspector;

        public FileTypeValidator()
        {
            var contentInspectorBuilder = new ContentInspectorBuilder()
            {
                Definitions = MimeDetective.Definitions.Default.All()           // Potentially need to review the definitions due to file types allowed.
            };
            
            _contentInspector = contentInspectorBuilder.Build();
        }

        /// <summary>
        /// Determine if content uploaded matches the file extension.
        /// See https://github.com/MediatedCommunications/Mime-Detective for usage.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public bool ContentMatchesExtension(Stream fileStream, string fileExtension)
        {
            if (fileStream is null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                throw new ArgumentNullException(nameof(fileExtension));
            }

            // Get the file content, this reads up to the first 10mb (header) of the file uploaded.
            var fileHeader = ContentReader.Default.ReadFromStream(fileStream, ResetPosition: true);

            // Inspect the file header
            var definitionMatches = _contentInspector.Inspect(fileHeader);

            var fileExtensionNoDot = fileExtension.StartsWith(".") ? fileExtension.Substring(1) : fileExtension;
            var fileExtensionWithDot = fileExtension.StartsWith(".") ? fileExtension : string.Concat('.', fileExtension);

            var isFileExtensionPresent = definitionMatches.ByFileExtension().Any(_ => fileExtensionNoDot.Equals(_.Extension, StringComparison.OrdinalIgnoreCase) || fileExtensionWithDot.Equals(_.Extension, StringComparison.OrdinalIgnoreCase)); 

            return isFileExtensionPresent;
        }
    }
}

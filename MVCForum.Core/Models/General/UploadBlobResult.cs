namespace MvcForum.Core.Models.General
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the result of uploading a file to blob storage.
    /// </summary>
    public sealed class UploadBlobResult
    {
        /// <summary>
        /// Constructor to initialise validations errors.
        /// </summary>
        public UploadBlobResult()
        {
        }

        /// <summary>
        /// Was the upload successful.
        /// </summary>
        public bool UploadSuccessful { get; set; } = false;

        /// <summary>
        /// Name of file uploaded to blob storage, generated as part of upload process
        /// and does not match original uploaded file name.
        /// </summary>
        public string UploadedFileName { get; set; }

        /// <summary>
        /// MD5 hash of uploaded file.
        /// </summary>
        public byte[] UploadedFileHash { get; set; }
    }
}

//-----------------------------------------------------------------------
// <copyright file="UploadBlobResult.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Models.General
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the result of uploading a file to blob storage.
    /// </summary>
    public class UploadBlobResult
    {
        /// <summary>
        /// Constructor to initialise validations errors.
        /// </summary>
        public UploadBlobResult()
        {
            UploadSuccessful = false;
            ValidationErrors = new List<string>();
        }

        /// <summary>
        /// Was the upload successful.
        /// </summary>
        public bool UploadSuccessful { get; set; }

        /// <summary>
        /// List of validation errors.
        /// </summary>
        public List<string> ValidationErrors { get; set; }

        /// <summary>
        /// Name of file uploaded to blob storage, generated as part of upload process
        /// and does not match original uploaded file name.
        /// </summary>
        public string UploadedFileName { get; set; }
    }
}

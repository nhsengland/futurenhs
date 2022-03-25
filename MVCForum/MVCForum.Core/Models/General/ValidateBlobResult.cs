namespace MvcForum.Core.Models.General
{
    using System.Collections.Generic;

    /// <summary>
    /// Used to return any blob upload validation messages.
    /// </summary>
    public sealed class ValidateBlobResult
    {
        /// <summary>
        /// List of validation errors.
        /// </summary>
        public IEnumerable<string> ValidationErrors { get; set; } = new List<string>();

        /// <summary>
        /// Correct mime type based on validating the file and doing a look up on the correct file extension.
        /// </summary>
        public string MimeType { get; set; }
    }
}

namespace MvcForum.Core.Models.Entities
{
    /// <summary>
    /// Defines the UploadStatus table entity.
    /// </summary>
    public class UploadStatus
    {
        /// <summary>
        /// Gets or sets the Id of the status (used to cast to enum).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the status.
        /// </summary>
        public string Name { get; set; }
    }
}

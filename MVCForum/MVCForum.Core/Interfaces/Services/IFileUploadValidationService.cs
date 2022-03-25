namespace MvcForum.Core.Interfaces.Services
{
    using MvcForum.Core.Models.General;
    using System.Web;

    public interface IFileUploadValidationService
    {
        ValidateBlobResult ValidateUploadedFile(HttpPostedFileBase file);
    }
}
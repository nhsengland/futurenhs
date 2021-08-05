//-----------------------------------------------------------------------
// <copyright file="IFileUploadValidationService.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Interfaces.Services
{
    using MvcForum.Core.Models.General;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    public interface IFileUploadValidationService
    {
        Task<UploadBlobResult> ValidateUploadedFile(HttpPostedFileBase file, bool simpleValidation);
    }
}
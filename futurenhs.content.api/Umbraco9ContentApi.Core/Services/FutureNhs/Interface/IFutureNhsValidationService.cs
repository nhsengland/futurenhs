﻿using Umbraco9ContentApi.Core.Models;

namespace Umbraco9ContentApi.Core.Services.FutureNhs.Interface
{
    public interface IFutureNhsValidationService
    {
        void ValidatePageContentModel(PageContentModel pageContentModel);
    }
}
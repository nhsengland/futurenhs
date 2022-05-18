﻿namespace Umbraco9ContentApi.Core.Composers
{
    using Microsoft.Extensions.DependencyInjection;
    using Services.FutureNhs.Interface;
    using Umbraco.Cms.Core.Composing;
    using Umbraco.Cms.Core.DependencyInjection;
    using Umbraco9ContentApi.Core.Converters;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs;
    using Umbraco9ContentApi.Core.Handlers.FutureNhs.Interface;
    using Umbraco9ContentApi.Core.Resolvers.Interfaces;
    using Umbraco9ContentApi.Core.Services.FutureNhs;
    using UmbracoContentApi.Core;
    using UmbracoContentApi.Core.Converters;
    using UmbracoContentApi.Core.Resolvers;

    /// <summary>
    /// Registers custom services on startup. 
    /// </summary>
    /// <seealso cref="IComposer" />
    public sealed class FutureNhsServicesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Services
            builder.Services.AddScoped<IFutureNhsContentService, FutureNhsContentService>();
            builder.Services.AddScoped<IFutureNhsSiteMapService, FutureNhsSiteMapService>();

            // Handlers
            builder.Services.AddScoped<IFutureNhsContentHandler, FutureNhsContentHandler>();
            builder.Services.AddScoped<IFutureNhsBlockHandler, FutureNhsBlockHandler>();
            builder.Services.AddScoped<IFutureNhsTemplateHandler, FutureNhsTemplateHandler>();
            builder.Services.AddScoped<IFutureNhsSiteMapHandler, FutureNhsSiteMapHandler>();

            // Converters
            builder.Converters().Replace<MultinodeTreepickerConverter, BlockPickerConverter>();
            builder.Converters().Replace<TextAreaConverter, JsonContentConverter>();

            // Resolvers
            builder.Services.AddTransient<IFutureNHSContentResolver, FutureNHSContentResolver>();
        }
    }
}

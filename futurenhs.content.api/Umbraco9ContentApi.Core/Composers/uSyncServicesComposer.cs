namespace Umbraco9ContentApi.Core.Composers
{
    using Microsoft.Extensions.DependencyInjection;
    using Umbraco.Cms.Core.Composing;
    using Umbraco.Cms.Core.DependencyInjection;
    using Umbraco9ContentApi.Core.Handlers.uSync;
    using Umbraco9ContentApi.Core.Handlers.uSync.Interface;
    using Umbraco9ContentApi.Core.Services;
    using Umbraco9ContentApi.Core.Services.uSync;
    using Umbraco9ContentApi.Core.Services.uSync.Interface;

    /// <summary>
    /// Registers custom services on startup. 
    /// </summary>
    /// <seealso cref="IComposer" />
    public sealed class uSyncServicesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddScoped<IuSyncHandler, uSyncHandler>();
            builder.Services.AddScoped<IuSyncContentService, uSyncContentService>();
            builder.Services.AddScoped<IuSyncContentTypeService, uSyncContentTypesService>();
            builder.Services.AddScoped<IuSyncDataTypeService, uSyncDataTypeService>();
            builder.Services.AddScoped<IuSyncMediaService, uSyncMediaService>();
            builder.Services.AddScoped<IuSyncMediaTypeService, uSyncMediaTypeService>();
        }
    }
}

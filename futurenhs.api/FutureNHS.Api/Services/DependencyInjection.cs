using FutureNHS.Api.Configuration;
using FutureNHS.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace FutureNHS.Api.Services
{
    public static class ServicesDependencyInjection
    {
        public static IServiceCollection Services(this IServiceCollection services)
        {
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IDiscussionService, DiscussionService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IGroupMembershipService, GroupMembershipService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupImageService, ImageService>();
            services.AddScoped<IFileServerService, FileServerService>();
            services.AddScoped<IUserImageService, ImageService>();
            services.AddScoped<IPermissionsService, PermissionsService>();
            services.AddScoped<IFileServerService, FileServerService>();
            //services.AddScoped<IFileServerService>(
            //    sp => {
            //        var fileServerConfig = sp.GetRequiredService<IOptionsSnapshot<FileServerTemplateUrlStrings>>().Value;

            //        if (fileServerConfig is null) throw new ApplicationException("Unable to load the file server configuration");
            //        if (string.IsNullOrWhiteSpace(fileServerConfig.TemplateUrl)) throw new ApplicationException("The template url is missing from the fileserver configuration section");
            //        if (string.IsNullOrWhiteSpace(fileServerConfig.TemplateUrlFileIdPlaceholder)) throw new ApplicationException("The file id placeholder is missing from the fileserver configuration section");

            //        var logger = sp.GetRequiredService<ILogger<FileServerService>>();

            //        return new FileServerService(fileServerConfig, logger);
            //    });

            return services;
        }
    }
}

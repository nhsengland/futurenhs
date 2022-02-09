namespace MvcForum.Core.Providers
{
    using MvcForum.Core.Interfaces.Providers;
    using System;

    public sealed class ConfigurationProvider : IConfigurationProvider
    {
        public string ReadOnlyDbConnectionString { get; }
        public string WriteOnlyDbConnectionString { get; }
        public string FileUploadConnectionString { get; }
        public string FileContainerName { get; }
        public string FileDownloadEndpoint { get; }
        public int RetryAttempts { get; }
        public int RetryDelay { get; }
        public string SmtpFrom { get; }
        public string FileServerTemplateUrl { get; }
        public string FileServerTemplateUrlFileIdPlaceholder { get; }
        public string ApplicationGatewayFqdn{ get; }

        public ConfigurationProvider(string readOnlyDbConnectionString, string writeOnlyDbConnectionString, 
                                     int retryAttempts, int retryDelay, string fileUploadConnectionString, 
                                     string fileContainerName, string fileDownloadEndpoint, string smtpFrom, 
                                     string fileServerTemplateUrl, string fileServerTemplateUrlFileIdPlaceholder,
                                     string applicationGatewayFqdn)
        {
            if (string.IsNullOrWhiteSpace(readOnlyDbConnectionString))
            {
                throw new ArgumentNullException(nameof(readOnlyDbConnectionString));
            }

            if (string.IsNullOrWhiteSpace(writeOnlyDbConnectionString))
            {
                throw new ArgumentNullException(nameof(writeOnlyDbConnectionString));
            }

            if (retryAttempts < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryAttempts));
            }

            if (retryDelay < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryDelay));
            }
            
            if (string.IsNullOrWhiteSpace(fileContainerName))
            {
                throw new ArgumentNullException(nameof(fileContainerName));
            }
            if (string.IsNullOrWhiteSpace(fileServerTemplateUrl))
            {
                throw new ArgumentNullException(nameof(fileServerTemplateUrl));
            }
            if (string.IsNullOrWhiteSpace(fileServerTemplateUrlFileIdPlaceholder))
            {
                throw new ArgumentNullException(nameof(fileServerTemplateUrlFileIdPlaceholder));
            }
         
            if (string.IsNullOrWhiteSpace(applicationGatewayFqdn))
            {
                throw new ArgumentNullException(nameof(applicationGatewayFqdn));
            }

            ReadOnlyDbConnectionString = readOnlyDbConnectionString;
            WriteOnlyDbConnectionString = writeOnlyDbConnectionString;
            FileUploadConnectionString = fileUploadConnectionString;
            FileContainerName = fileContainerName;
            FileDownloadEndpoint = fileDownloadEndpoint;
            RetryAttempts = retryAttempts;
            RetryDelay = retryDelay;
            SmtpFrom = smtpFrom;
            FileServerTemplateUrl = fileServerTemplateUrl;
            FileServerTemplateUrlFileIdPlaceholder = fileServerTemplateUrlFileIdPlaceholder;
            ApplicationGatewayFqdn = applicationGatewayFqdn;
        }
    }
}

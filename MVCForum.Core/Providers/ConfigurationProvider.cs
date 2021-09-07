//-----------------------------------------------------------------------
// <copyright file="ConfigurationProvider.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Providers
{
    using MvcForum.Core.Interfaces.Providers;

    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly string _readOnlyDbConnectionString;
        private readonly string _fileUploadConnectionString;
        private readonly string _fileContainerName;
        private readonly string _fileDownloadEndpoint;
        private readonly int _retryAttempts;
        private readonly int _retryDelay;
        private readonly string _smtpFrom;

        public ConfigurationProvider(string readOnlyDbConnectionString, int retryAttempts, int retryDelay, string fileUploadConnectionString, string fileContainerName, string fileDownloadEndpoint, string smtpFrom)
        {
            _readOnlyDbConnectionString = readOnlyDbConnectionString;
            _fileUploadConnectionString = fileUploadConnectionString;
            _fileContainerName = fileContainerName;
            _fileDownloadEndpoint = fileDownloadEndpoint;
            _retryAttempts = retryAttempts;
            _retryDelay = retryDelay;
            _smtpFrom = smtpFrom;
        }

        public string GetReadOnlyConnectionString()
        {
            return _readOnlyDbConnectionString;
        }

        public string GetFileUploadConnectionString()
        {
            return _fileUploadConnectionString;
        }

        public string GetFileContainerName()
        {
            return _fileContainerName;
        }

        public string GetFileDownloadEndpoint()
        {
            return _fileDownloadEndpoint;
        }

        public int GetRetryAttempts()
        {
            return _retryAttempts;
        }

        public int GetRetryDelay()
        {
            return _retryDelay;
        }

        public string GetSmtpFrom()
        {
            return _smtpFrom;
        }
    }
}

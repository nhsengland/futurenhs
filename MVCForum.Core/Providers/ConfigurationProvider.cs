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
        private readonly string _writeDbConnectionString;
        private readonly int _retryAttempts;
        private readonly int _retryDelay;

        public ConfigurationProvider(string readOnlyDbConnectionString, string writeDbConnectionString, int retryAttempts, int retryDelay)
        {
            _readOnlyDbConnectionString = readOnlyDbConnectionString;
            _writeDbConnectionString = writeDbConnectionString;
            _retryAttempts = retryAttempts;
            _retryDelay = retryDelay;
        }

        public string GetReadOnlyConnectionString()
        {
            return _readOnlyDbConnectionString;
        }

        public string GetWriteConnectionString()
        {
            return _writeDbConnectionString;
        }

        public int GetRetryAttempts()
        {
            return _retryAttempts;
        }

        public int GetRetryDelay()
        {
            return _retryDelay;
        }
    }
}

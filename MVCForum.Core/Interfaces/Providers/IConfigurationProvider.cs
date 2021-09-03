//-----------------------------------------------------------------------
// <copyright file="IConfigurationProvider.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Interfaces.Providers
{
    public interface IConfigurationProvider
    {
        string WriteOnlyDbConnectionString { get; }
        string ReadOnlyDbConnectionString { get; }
        string FileUploadConnectionString { get; }
        string FileContainerName { get; }
        string FileDownloadEndpoint { get; }
        int RetryAttempts { get; }
        int RetryDelay { get; }
        string SmtpFrom { get; }
    }
}

//-----------------------------------------------------------------------
// <copyright file="IConfigurationProvider.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Interfaces.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IConfigurationProvider
    {
        string GetReadOnlyConnectionString();
        string GetWriteConnectionString();
        int GetRetryAttempts();
        int GetRetryDelay();
    }
}

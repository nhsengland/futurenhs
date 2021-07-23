//-----------------------------------------------------------------------
// <copyright file="IDbConnectionFactory.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Database.DatabaseProviders.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDbConnectionFactory
    {
        IDbConnection CreateReadOnlyConnection();
        IDbConnection CreateWriteConnection();
    }
}

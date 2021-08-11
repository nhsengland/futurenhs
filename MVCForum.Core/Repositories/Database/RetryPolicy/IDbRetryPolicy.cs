//-----------------------------------------------------------------------
// <copyright file="IDbRetryPolicy.cs" company="CDS">
// Copyright (c) CDS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MvcForum.Core.Repositories.Database.RetryPolicy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDbRetryPolicy
    {
        void Execute(Action operation);

        TResult Execute<TResult>(Func<TResult> operation);

        Task Execute(Func<Task> operation, CancellationToken cancellationToken);

        Task<TResult> Execute<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken);
    }
}

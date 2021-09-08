namespace MvcForum.Core.Repositories.Database.RetryPolicy
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDbRetryPolicy
    {
        void Execute(Action operation);

        TResult Execute<TResult>(Func<TResult> operation);

        Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken);

        Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken);
    }
}

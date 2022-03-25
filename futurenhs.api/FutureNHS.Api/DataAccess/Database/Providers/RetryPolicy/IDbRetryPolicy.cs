using Polly.Wrap;

namespace FutureNHS.Api.DataAccess.Database.Providers.RetryPolicy
{
    public interface IDbRetryPolicy
    {
        AsyncPolicyWrap RetryPolicyAsync { get; }
        PolicyWrap RetryPolicy { get; }
        //void Execute(Action operation);
        //TResult Execute<TResult>(Func<TResult> operation);

        //Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken);

        //Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken);
        ////Task<object> ExecuteAsync(Task<object> executeScalarAsync);
        //Task<Task> ExecuteAsync<TResult>(Task operation, CancellationToken cancellationToken);
    }
}

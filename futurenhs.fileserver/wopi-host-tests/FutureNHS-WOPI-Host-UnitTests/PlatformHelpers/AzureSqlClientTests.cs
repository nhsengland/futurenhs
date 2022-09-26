using FutureNHS.WOPIHost.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]  // Helps us to assure we are writing thread safe code

namespace FutureNHS_WOPI_Host_UnitTests.PlatformHelpers
{
    [TestClass]
    public sealed class AzureSqlClientTests
    {
#if DEBUG
        [TestMethod]
        public async Task AsyncResiliencyPolicy_IsUnemployedWhenWorkSucceeds()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<AzureSqlClient>>().Object;

            var sqlCnFactoryLogger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            var sqlDbConnectionFactory = new AzureSqlDbConnectionFactory("read-write", "read-only", sqlCnFactoryLogger);

            var azureSqlClient = new AzureSqlClient(sqlDbConnectionFactory, logger);

            var retryPolicy = azureSqlClient.GetAsyncRetryPolicy();

            var policyKey = Guid.NewGuid().ToString();

            var globalResiliencyPolicy = azureSqlClient.GetAsyncGlobalResiliencyPolicyFor(policyKey);

            var policy = Policy.WrapAsync(retryPolicy, globalResiliencyPolicy);

            var policyResult = await policy.ExecuteAndCaptureAsync(ct => Task.CompletedTask, cancellationToken);

            Assert.AreEqual(OutcomeType.Successful, policyResult.Outcome);

            Assert.IsNull(policyResult.ExceptionType);
            Assert.IsNull(policyResult.FinalException);
        }




        [TestMethod]
        public async Task RetryPolicy_RecoversSuccessfullyWhenWorkSucceedsAfterRetryUponTransientException()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<AzureSqlClient>>().Object;

            var sqlCnFactoryLogger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            var sqlDbConnectionFactory = new AzureSqlDbConnectionFactory("read-write", "read-only", sqlCnFactoryLogger);

            var azureSqlClient = new AzureSqlClient(sqlDbConnectionFactory, logger);

            var retryPolicy = azureSqlClient.GetAsyncRetryPolicy();

            var retryCount = 0;

            var policyResult = await retryPolicy.ExecuteAndCaptureAsync(
                ct => ++retryCount > 1 ? Task.CompletedTask : throw new TimeoutException(), 
                cancellationToken);

            Assert.AreEqual(2, retryCount);

            Assert.AreEqual(OutcomeType.Successful, policyResult.Outcome);

            Assert.IsNull(policyResult.ExceptionType);
            Assert.IsNull(policyResult.FinalException);
        }

        [TestMethod]
        public async Task RetryPolicy_FailsWhenWorkFailsWithNonTransientException()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<AzureSqlClient>>().Object;

            var sqlCnFactoryLogger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            var sqlDbConnectionFactory = new AzureSqlDbConnectionFactory("read-write", "read-only", sqlCnFactoryLogger);

            var azureSqlClient = new AzureSqlClient(sqlDbConnectionFactory, logger);

            var retryPolicy = azureSqlClient.GetAsyncRetryPolicy();

            var retryCount = 0;

            var policyResult = await retryPolicy.ExecuteAndCaptureAsync(
                ct => ++retryCount > 1 ? Task.CompletedTask : throw new ApplicationException(),
                cancellationToken);

            Assert.AreEqual(1, retryCount);

            Assert.AreEqual(OutcomeType.Failure, policyResult.Outcome);

            Assert.IsNotNull(policyResult.ExceptionType);
            Assert.IsNotNull(policyResult.FinalException);

            Assert.AreEqual(ExceptionType.Unhandled, policyResult.ExceptionType.Value);
            Assert.IsInstanceOfType(policyResult.FinalException, typeof(ApplicationException));
        }

        [TestMethod]
        public async Task RetryPolicy_FailsAfterMaxAttemptsAfterRetryOnTransientExceptions()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<AzureSqlClient>>().Object;

            var sqlCnFactoryLogger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            var sqlDbConnectionFactory = new AzureSqlDbConnectionFactory("read-write", "read-only", sqlCnFactoryLogger);

            var azureSqlClient = new AzureSqlClient(sqlDbConnectionFactory, logger);

            var retryPolicy = azureSqlClient.GetAsyncRetryPolicy();

            var retryCount = 0;

            var policyResult = await retryPolicy.ExecuteAndCaptureAsync(
                ct => { retryCount++; throw new TimeoutException(); },
                cancellationToken);

            Assert.AreEqual(1 + 5, retryCount);

            Assert.AreEqual(OutcomeType.Failure, policyResult.Outcome);

            Assert.IsNotNull(policyResult.ExceptionType);
            Assert.IsNotNull(policyResult.FinalException);

            Assert.AreEqual(ExceptionType.HandledByThisPolicy, policyResult.ExceptionType.Value); // Exception type the policy handles but too many of them to keep going
            Assert.IsInstanceOfType(policyResult.FinalException, typeof(TimeoutException));
        }

        [TestMethod]
        public async Task RetryPolicy_EmploysExponentialBackoffUponTransientExceptions()
        {
            var cancellationToken = new CancellationToken();

            var logger = new Moq.Mock<ILogger<AzureSqlClient>>().Object;

            var sqlCnFactoryLogger = new Moq.Mock<ILogger<AzureSqlDbConnectionFactory>>().Object;

            var sqlDbConnectionFactory = new AzureSqlDbConnectionFactory("read-write", "read-only", sqlCnFactoryLogger);

            var azureSqlClient = new AzureSqlClient(sqlDbConnectionFactory, logger);

            var retryPolicy = azureSqlClient.GetAsyncRetryPolicy();

            var retryCount = 0;

            var xs = new TimeSpan[6];

            var sw = new Stopwatch();
            
            sw.Start();

            var policyResult = await retryPolicy.ExecuteAndCaptureAsync(
                ct => { xs[retryCount++] = sw.Elapsed; throw new TimeoutException(); },
                cancellationToken);

            sw.Stop();

            // First delay in array can be ignored because it's just the amount of time until the first invocation - ie not a retry

            var firstDelay = xs[1] - xs[0];
            var secondDelay = xs[2] - xs[1];
            var thirdDelay = xs[3] - xs[2];
            var fourthDelay = xs[4] - xs[3];
            var fifthDelay = xs[5] - xs[4];

            // Interestingly, it seems expoenential + jitter in Polly land doesn't actually mean exponential .. just pretty close.
            // Could be a side effect of what it uses to time/sleep (ie best efforts), but in all practicality it doesn't actually make
            // and difference to how we are using it; just makes it harder to test

            const int TOLERANCE_IN_MS = 1000;

            Assert.IsTrue(thirdDelay.TotalMilliseconds > (secondDelay * 2).TotalMilliseconds - TOLERANCE_IN_MS, $"Expected third delay '{thirdDelay}' to be at least twice as long as second '{secondDelay}'");
            Assert.IsTrue(fourthDelay.TotalMilliseconds > (thirdDelay * 2).TotalMilliseconds - TOLERANCE_IN_MS, $"Expected fourth delay '{fourthDelay}' to be at least twice as long as third '{thirdDelay}'");
            Assert.IsTrue(fifthDelay.TotalMilliseconds > (fourthDelay * 2).TotalMilliseconds - TOLERANCE_IN_MS, $"Expected fifth delay '{fifthDelay}' to be at least twice as long as fourth '{fourthDelay}'");
        }

        // TODO - check cancellaton token is honoured and terminates retry block early while not affecting other policies
        // TODO - check cancellaton token is honoured and terminates when waiting on circuit breaker
        // TODO - check cancellaton token is honoured and terminates when queued in bulkhead

        // Pull out a combination of a bulkhead and circuit breaker and test it works as expected:
        //
        // A bulkhead is used to limit the amount of concurrent traffic our application generates for something downstream
        // thus protecting it from issues in our system causing problems in theirs - we are a good citizen if you will!
        // 
        // We use a circuit breaker to fail early in our application if we have a strong suspicion a downstream application is 
        // struggling.  The idea to for us to back off for a while to give it time to recover, rather than have us throwing ever
        // more traffic in its direction by way of transient retries.

        [TestMethod]
        public void ResiliencyPolicy_BulkheadQueuesExcessLoadAndThrowsOutOldersWorkWhenTooMuchInQueue()
        {
            var logger = new Moq.Mock<ILogger<AzureSqlClient>>();

            var azureSqlDbConnectionFactory = new Moq.Mock<IAzureSqlDbConnectionFactory>();

            var azureSqlClient = new AzureSqlClient(azureSqlDbConnectionFactory.Object, logger.Object);

            var policy = azureSqlClient.GetAsyncBulkheadPolicy();

            // Bulkhead is configured for a max concurrent rate of 3 with max queue size of 25
            // In this test we will prevent all tasks from completing apart from the first, thus we should expect to see 
            // a max number of executing tasks of 3, 1 completion, 25 queuing and 4 rejections

            const bool SIGNALED = true;

            var gate = new AutoResetEvent(initialState: SIGNALED);

            var invocations = 0;
            var completed = 0;

            var xs = new Task<PolicyResult>[50];

            var cancellationToken = new CancellationToken();

            for (var n = 0; n < 50; n++)
            {
                var root = Task.Run(
                    () => 
                        policy.ExecuteAndCaptureAsync(
                            ct => {
                                invocations++;

                                var signalled = gate.WaitOne(10);

                                if (signalled)
                                {
                                    completed++;

                                    return Task.CompletedTask;
                                }
                                else
                                {
                                    return Task.Delay(-1, ct);
                                }
                            }, cancellationToken), 
                        cancellationToken);

                xs[n] = root;
            }

            Task.WaitAll(xs, 1000, cancellationToken);

            // 1 task should have completed as the gate was open to the first past the post
            // 25 + 3 tasks should still be working (max queue size + max concurrency)
            // 50 - (1 + 25 + 3) tasks should have been rejected by the policy

            Assert.AreEqual(1, completed, "Expected just one task to complete");         

            var policyResults = xs.Where(_ => _.IsCompleted).Select(_ => _.Result).ToArray();

            var failures = policyResults.Where(_ => _.Outcome == OutcomeType.Failure).ToArray();

            Assert.AreEqual(21, failures.Length, "Expected 21 work items to have failed to complete");  

            var rejections = failures.Where(_ => _.FinalException.GetType() == typeof(Polly.Bulkhead.BulkheadRejectedException)).Count();

            Assert.AreEqual(21, rejections, "Expected the bulkhead policy to reject 21 work items due to being full");   
        }
#endif
    }
}

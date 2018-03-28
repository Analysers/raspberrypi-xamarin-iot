using System;
using System.Threading.Tasks;
using Polly;

namespace IotApp.Helpers
{
    public static class ResilientCall
    {
        public static async Task<PolicyResult<TReturn>> ExecuteWithRetry<TReturn>(Func<Task<TReturn>> action, int retryCount = 2)
        {
            return await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync
                (
                    retryCount: retryCount,
                    sleepDurationProvider: retryAttempt => retryAttempt.OnExponentially()
                )
                .ExecuteAndCaptureAsync(
                    () => action()
                ).ConfigureAwait(false);
        }

        private static TimeSpan OnExponentially(this int retryAttempt)
        {
            return TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 1000 / 4);
        }
    }
}

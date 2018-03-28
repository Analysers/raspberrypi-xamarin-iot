using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Appliance.Helpers
{
    public static class ResilientCall
    {
        public static async Task<PolicyResult<HttpResponseMessage>> ExecuteWithRetry(Func<Task<HttpResponseMessage>> action, int retryCount = 2)
        {
            HttpStatusCode[] httpStatusCodesWorthRetrying = {
                HttpStatusCode.RequestTimeout, // 408
                HttpStatusCode.InternalServerError, // 500
                HttpStatusCode.BadGateway, // 502
                HttpStatusCode.ServiceUnavailable, // 503
                HttpStatusCode.GatewayTimeout // 504
            };

            return await Policy
                .HandleResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                .WaitAndRetryAsync
                (
                    retryCount: retryCount,
                    sleepDurationProvider: retryAttempt => retryAttempt.OnExponentially()
                )
                .ExecuteAndCaptureAsync(
                    async () => await action()
                ).ConfigureAwait(false);
        }

        private static TimeSpan OnExponentially(this int retryAttempt)
        {
            return TimeSpan.FromSeconds(Math.Pow(retryAttempt, 1));
        }
    }
}

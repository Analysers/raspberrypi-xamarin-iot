using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Easy.Common.Interfaces;
using Appliance.Helpers;
using Appliance.Models.Ring;
using Newtonsoft.Json;
using Polly;
using Serilog;

namespace Appliance.Services
{
    /// <summary>
    /// Poll Ring's API every 5 seconds to get any active dings.
    /// 5 seconds is the maximum recommended frequency. I believe the Ring app uses 5 seconds, 
    /// and dings dissapear after 5 seconds so you'll lose dings if you poll later than 5 seconds.
    /// On first call, we get a ring auth token, which we then reuse for subsequent polls.
    /// </summary>
    public class RingService : IRingService
    {
        private const string ActiveDingsEndpointUrl = "https://api.ring.com/clients_api/dings/active?auth_token={0}&api_version=9";
        private const string SessionEndpointUrl = "https://api.ring.com/clients_api/session";
        private static string _authenticationToken = "";
        private readonly IRestClient _restClient;

        public RingService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<ActiveDing[]> PollActiveDings(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_authenticationToken))
            {
                var tokenSuccess = await GetRingToken(cancellationToken);
                if (tokenSuccess)
                    return await PollActiveDings(cancellationToken);
                
                return new List<ActiveDing>().ToArray();
            }

            try
            {
                var req = await ResilientCall.ExecuteWithRetry(
                    async () => await _restClient.GetAsync(string.Format(ActiveDingsEndpointUrl, _authenticationToken), cancellationToken),
                    retryCount: 1
                );

                if (req.Outcome != OutcomeType.Successful)
                {
                    if (req.FinalHandledResult.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        var tokenSuccess = await GetRingToken(cancellationToken);
                        if (tokenSuccess)
                            return await PollActiveDings(cancellationToken);
                    }

                    Log.Error(req.FinalException, "Error getting active dings");
                    return new List<ActiveDing>().ToArray();
                }

                var resp = await req.Result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ActiveDing[]>(resp, Config.JsonSettings);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting active dings");
                return new List<ActiveDing>().ToArray();
            }
        }

        private async Task<bool> GetRingToken(CancellationToken cancellationToken)
        {
            try
            {
                var response = await ResilientCall.ExecuteWithRetry(
                    async () =>
                    {
                        var request =
                            new HttpRequestMessage(HttpMethod.Post, SessionEndpointUrl)
                            {
                                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("device[os]", "ios"),
                                    new KeyValuePair<string, string>("device[hardware_id]", Config.RingHardwareId),
                                    new KeyValuePair<string, string>("api_version", "9")
                                })
                            };
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Config.RingBasicAuth);

                        return await _restClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
                    },
                    retryCount: 1
                );

                if (response.Outcome != OutcomeType.Successful)
                {
                    Log.Error(response.FinalException, "Error getting ring token");
                    return false;
                }

                var resp = await response.Result.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<Session>(resp, Config.JsonSettings);

                _authenticationToken = res.Profile.AuthenticationToken;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting ring token");
                return false;
            }
        }
    }
}

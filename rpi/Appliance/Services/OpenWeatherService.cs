using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Easy.Common.Interfaces;
using Appliance.Commands;
using Appliance.Controllers;
using Appliance.Domain;
using Appliance.Events;
using Appliance.Helpers;
using Appliance.Models.OpenWeather;
using MediatR;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Polly;
using Serilog;

namespace Appliance.Services
{
    /// <summary>
    /// Call open weather to get sunset and sunrise times for the day.
    /// Create OnSunrise and OnSunset timer events to trigger on those times.
    /// Also create an OnLightsOff timer event to turn on garden lights just after midnight (between 12:00am and 12:30am).
    /// If the appliance starts, and it's during a time where garden lights should be on, turn them on.
    /// </summary>
    public class OpenWeatherService : IOpenWeatherService
    {
        private readonly string _openWeatherApiUrl = $"https://api.openweathermap.org/data/2.5/weather?zip=3152,au&units=metric&APPID={Config.OpenWeatherAppId}";
        private readonly IMediator _mediator;
        private readonly IClock _clock;
        private readonly IRestClient _restClient;
        private readonly ILightsController _lightsController;
        private readonly ITimerEvents _timerEvents;

        public OpenWeatherService(IMediator mediator, IClock clock, IRestClient restClient, ILightsController lightsController, ITimerEvents timerEvents)
        {
            _mediator = mediator;
            _clock = clock;
            _restClient = restClient;
            _lightsController = lightsController;
            _timerEvents = timerEvents;
        }

        public async Task UpdateWeatherData(CancellationToken cancellationToken)
        {
            int LightsOffMinutesUnder30(TimeSpan timeSpan)
            {
                return timeSpan.Minutes > 30 ? timeSpan.Minutes - 30 : timeSpan.Minutes;
            }

            bool LightsOffEnabled(TimeSpan sunset1, TimeSpan sunrise1)
            {
                return _clock.Now.Within(sunset1, sunrise1);
            }

            bool ShouldLightsBeOn(TimeSpan sunset)
            {
                return _clock.Now.TimeSpan() > sunset;
            }

            try
            {
                var response = await ResilientCall.ExecuteWithRetry(
                    async () => await _restClient.GetAsync(_openWeatherApiUrl, cancellationToken)
                );

                if (response.Outcome != OutcomeType.Successful)
                {
                    Log.Error(response.FinalException, "Error getting weather data");
                    return;
                }

                var content = await response.Result.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<OpenWeatherResult>(content, Config.JsonSettings);

                var sunrise = new TimedEvent(result.Sys.Sunrise.LocalTimeSpanFromUnixTime(), true);
                var sunset = new TimedEvent(result.Sys.Sunset.LocalTimeSpanFromUnixTime(), true);
                var lightsOff = new TimedEvent(new TimeSpan(00, LightsOffMinutesUnder30(sunset.TriggerTime), 00), LightsOffEnabled(sunset.TriggerTime, sunrise.TriggerTime));

                if (ShouldLightsBeOn(sunset.TriggerTime))
                    _lightsController.OnSunset();

                var eventsToUpdate = new ConcurrentDictionary<Enums.TimedEvent, TimedEvent>();

                var onSunriseEvent = eventsToUpdate.AddOrUpdate(Enums.TimedEvent.OnSunrise, sunrise, (key, oldValue) => sunrise);
                var onSunsetEvent = eventsToUpdate.AddOrUpdate(Enums.TimedEvent.OnSunset, sunset, (key, oldValue) => sunset);
                var onLightsOffEvent = eventsToUpdate.AddOrUpdate(Enums.TimedEvent.OnLightsOff, lightsOff, (key, oldValue) => lightsOff);

                foreach (var evnt in eventsToUpdate)
                {
                    _timerEvents.UpdateEvent(evnt.Key, evnt.Value);
                }

                await _mediator.Send(new UpdateReportedPropertiesCommand(new TwinCollection
                {
                    [nameof(Enums.TimedEvent.OnSunrise)] = onSunriseEvent,
                    [nameof(Enums.TimedEvent.OnSunset)] = onSunsetEvent,
                    [nameof(Enums.TimedEvent.OnLightsOff)] = onLightsOffEvent
                }), cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating weather data");
            }
        }
    }
}

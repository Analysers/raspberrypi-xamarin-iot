using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Easy.Common.Interfaces;
using Appliance.Commands;
using Appliance.Components;
using Appliance.Controllers;
using Appliance.Domain;
using Appliance.Events;
using Appliance.Helpers;
using MediatR;
using Serilog;
using Relay = Appliance.Enums.Relay;

namespace Appliance.Services
{
    /// <summary>
    /// The timer service that loops through each eanbled timed event, and acts on it.
    /// Using the Easy.Common.TimerClock.Tick event which is raised every second.
    /// Toggle the LED every tick event for a visible appliance heartbeat.
    /// Poll doorbell motion every 5 seconds.
    /// Send a silent push notification every hour.
    /// </summary>
    public class TimerService : ITimerService
    {
        private readonly ITimerClock _timerClock;
        private readonly IClock _clock;
        private readonly IMediator _mediator;
        private readonly ILedBoard _ledBoard;
        private readonly ITimerEvents _timerEvents;
        private readonly ILightsController _lightsController;
        private readonly IRelayBoard _relayBoard;

        public TimerService(ITimerClock timerClock, IClock clock, IMediator mediator, ILedBoard ledBoard, ITimerEvents timerEvents, ILightsController lightsController, IRelayBoard relayBoard)
        {
            _timerClock = timerClock;
            _clock = clock;
            _mediator = mediator;
            _ledBoard = ledBoard;
            _timerEvents = timerEvents;
            _lightsController = lightsController;
            _relayBoard = relayBoard;
        }

        public void Start()
        {
            _timerClock.Tick += async (sender, eventArgs) =>
            {
                ToggleLed();
                
                var timeSpanNow = _clock.Now.TimeSpan();
                await CheckForTriggeredEvents(timeSpanNow);
                await PollTimedEvents(timeSpanNow);
            };

            _timerClock.Enabled = true;
            Log.Information("Timer has started");
        }

        private async Task PollTimedEvents(TimeSpan timeSpanNow)
        {
            bool PollDoorbellMotion()
            {
                return false; // Turn on if you have a Ring doorbell and update settings in Config.cs
                // Every 5 seconds
                return DateTimeHelpers.TimeFallsOnThe5SecondMark(timeSpanNow);
            }

            if (PollDoorbellMotion())
            {
                await _mediator.Publish(new PollDoorbellMotionCommand());
            }
        }

        private async Task CheckForTriggeredEvents(TimeSpan timeSpanNow)
        {
            bool TriggeredEvent(KeyValuePair<Enums.TimedEvent, TimedEvent> evt)
            {
                return evt.Value.TriggerTime == timeSpanNow && evt.Value.Enabled;
            }

            void DisableEvent(KeyValuePair<Enums.TimedEvent, TimedEvent> evt)
            {
                _timerEvents.DisableEvent(evt.Key);
            }

            foreach (var evt in _timerEvents.GetEnabledEvents())
            {
                switch (evt.Key)
                {
                    case Enums.TimedEvent.UpdateSunsetSunrise:
                        if (TriggeredEvent(evt))
                        {
                            await _mediator.Publish(new UpdateSunriseSunsetCommand());
                        }

                        break;
                    case Enums.TimedEvent.OnSunset:
                        if (TriggeredEvent(evt))
                        {
                            _lightsController.OnSunset();
                        }

                        break;
                    case Enums.TimedEvent.OnLightsOff:
                        if (TriggeredEvent(evt))
                        {
                            await _lightsController.LightsOff();
                        }

                        break;
                    case Enums.TimedEvent.OnSunrise:
                        if (TriggeredEvent(evt))
                        {
                            _lightsController.OnSunrise();
                        }

                        break;
                    case Enums.TimedEvent.StrobeOff:
                        if (TriggeredEvent(evt))
                        {
                            _relayBoard.Off(Relay.AlarmStrobe);
                            DisableEvent(evt);
                        }

                        break;
                    case Enums.TimedEvent.SirenOff:
                        if (TriggeredEvent(evt))
                        {
                            _relayBoard
                                .Relay(Relay.AlarmSiren)
                                .Off();
                            DisableEvent(evt);
                        }
                        break;
                    case Enums.TimedEvent.GarageDoorOperatedEnd:
                        if (TriggeredEvent(evt))
                        {
                            Config.GarageDoorOperated = false;
                            DisableEvent(evt);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void ToggleLed()
        {
            _ledBoard.Toggle(Enums.Led.Heartbeat);
        }
    }
}

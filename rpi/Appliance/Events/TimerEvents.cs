using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Easy.Common.Extensions;
using Easy.Common.Interfaces;
using Appliance.Helpers;
using Serilog;
using Enum = System.Enum;

namespace Appliance.Events
{
    /// <summary>
    /// Create the initial timers events with default values if any
    /// </summary>
    public class TimerEvents : ITimerEvents
    {
        private static Domain.TimedEvent EmptyTimedEvent => new Domain.TimedEvent();
        private readonly ConcurrentDictionary<Enums.TimedEvent, Domain.TimedEvent> _events;

        public TimerEvents(IClock clock)
        {
            void CreateDefaultEvents()
            {
                const string couldNotAddErr = "Could not add {0} to ConcurrentDictionary";
                var defaultSunriseTime = new TimeSpan(05, 30, 00);
                var defaultSunsetTime = new TimeSpan(19, 30, 00);

                foreach (Enums.TimedEvent timedEvent in Enum.GetValues(typeof(Enums.TimedEvent)))
                {
                    var err = string.Format(couldNotAddErr, timedEvent);

                    switch (timedEvent)
                    {
                        case Enums.TimedEvent.UpdateSunsetSunrise:
                            if (!_events.TryAdd(timedEvent, new Domain.TimedEvent(new TimeSpan(02, 00, 00), false)))
                                Log.Error(new Exception(err), err);

                            break;
                        case Enums.TimedEvent.OnSunrise:
                            if (!_events.TryAdd(timedEvent, new Domain.TimedEvent(defaultSunriseTime, false)))
                                Log.Error(new Exception(err), err);

                            break;
                        case Enums.TimedEvent.OnSunset:
                            if (!_events.TryAdd(timedEvent, new Domain.TimedEvent(defaultSunsetTime, false)))
                                Log.Error(new Exception(err), err);

                            break;
                        case Enums.TimedEvent.OnLightsOff:
                            var enabled = clock.Now.Within(defaultSunsetTime, defaultSunriseTime);

                            if (!_events.TryAdd(timedEvent, new Domain.TimedEvent(clock.Now.AddTimeSpan(15.Minutes()), enabled)))
                                Log.Error(new Exception(err), err);

                            break;
                        default:
                            _events.TryAdd(timedEvent, EmptyTimedEvent);
                            break;
                    }
                }
            }

            _events = new ConcurrentDictionary<Enums.TimedEvent, Domain.TimedEvent>();
            CreateDefaultEvents();
        }

        public IEnumerable<KeyValuePair<Enums.TimedEvent, Domain.TimedEvent>> GetEnabledEvents()
        {
            return _events.Where(x => x.Value.Enabled);
        }

        public Domain.TimedEvent UpdateEvent(Enums.TimedEvent eventType, Domain.TimedEvent evnt)
        {
            _events.AddOrUpdate(eventType, evnt, (key, oldValue) => evnt);
            return evnt;
        }

        public Domain.TimedEvent DisableEvent(Enums.TimedEvent eventType)
        {
            if (_events.TryGetValue(eventType, out var evnt))
            {
                evnt.Disable();
                _events.AddOrUpdate(eventType, evnt, (key, oldValue) => evnt);
                return evnt;
            }
            else
            {
                var err = $"Failed to disable event {eventType}";
                Log.Error(new Exception(err), err);
                return null;
            }
        }

        public Domain.TimedEvent GetEvent(Enums.TimedEvent eventType)
        {
            return _events[eventType];
        }
    }
}

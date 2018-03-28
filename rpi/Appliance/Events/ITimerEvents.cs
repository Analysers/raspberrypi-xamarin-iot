using System.Collections.Generic;

namespace Appliance.Events
{
    public interface ITimerEvents
    {
        IEnumerable<KeyValuePair<Enums.TimedEvent, Domain.TimedEvent>> GetEnabledEvents();
        Domain.TimedEvent UpdateEvent(Enums.TimedEvent eventType, Domain.TimedEvent @event);
        Domain.TimedEvent DisableEvent(Enums.TimedEvent eventType);
        Domain.TimedEvent GetEvent(Enums.TimedEvent eventType);
    }
}
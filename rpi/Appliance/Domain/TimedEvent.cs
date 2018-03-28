using System;

namespace Appliance.Domain
{
    public class TimedEvent
    {
        public TimeSpan TriggerTime { get; private set; }
        public bool Enabled { get; private set; }

        public TimedEvent()
        {
            TriggerTime = new TimeSpan(0);
            Enabled = false;
        }

        public TimedEvent(TimeSpan triggerTime, bool enabled)
        {
            TriggerTime = triggerTime;
            Enabled = enabled;
        }

        public void Enable()
        {
            Enabled = true;
        }

        public void Disable()
        {
            Enabled = false;
        }

        public void SetTriggerTime(TimeSpan triggerTime)
        {
            TriggerTime = triggerTime;
        }
    }
}

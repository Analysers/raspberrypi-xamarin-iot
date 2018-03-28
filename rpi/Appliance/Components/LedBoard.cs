using System;

namespace Appliance.Components
{
    public class LedBoard : ILedBoard
    {
        private readonly Led _heartbeat;

        public LedBoard()
        {
            _heartbeat = new Led(Enums.Led.Heartbeat);
        }

        public bool IsOn(Enums.Led led)
        {
            switch (led)
            {
                case Enums.Led.Heartbeat:
                    return _heartbeat.State;
                default:
                    throw new ArgumentOutOfRangeException(nameof(led), led, null);
            }
        }

        public void Off(Enums.Led led)
        {
            switch (led)
            {
                case Enums.Led.Heartbeat:
                    _heartbeat.State = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(led), led, null);
            }
        }

        public void On(Enums.Led led)
        {
            switch (led)
            {
                case Enums.Led.Heartbeat:
                    _heartbeat.State = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(led), led, null);
            }
        }

        public void Toggle(Enums.Led led)
        {
            switch (led)
            {
                case Enums.Led.Heartbeat:
                    _heartbeat.State = !_heartbeat.State;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(led), led, null);
            }
        }
    }
}

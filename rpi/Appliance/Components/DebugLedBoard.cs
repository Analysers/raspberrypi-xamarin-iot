using System;
using Serilog;

namespace Appliance.Components
{
    public class DebugLedBoard : ILedBoard
    {
        private bool _heartbeat;

        public DebugLedBoard()
        {
            _heartbeat = false;
        }

        public bool IsOn(Enums.Led led)
        {
            const string logInfo = "Led {0} State: {1}";

            switch (led)
            {
                case Enums.Led.Heartbeat:
                    Log.Information(string.Format(logInfo, led, _heartbeat));
                    return _heartbeat;
                default:
                    throw new ArgumentOutOfRangeException(nameof(led), led, null);
            }
        }

        public void Off(Enums.Led led)
        {
            switch (led)
            {
                case Enums.Led.Heartbeat:
                    _heartbeat = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(led), led, null);
            }

            Log.Information($"Led Off: {led}");
        }

        public void On(Enums.Led led)
        {
            switch (led)
            {
                case Enums.Led.Heartbeat:
                    _heartbeat = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(led), led, null);
            }

            Log.Information($"Led On: {led}");
        }

        public void Toggle(Enums.Led led)
        {
            const string logInfo = "Led {0} Toggle, State: {1}";

            switch (led)
            {
                case Enums.Led.Heartbeat:
                    _heartbeat = !_heartbeat;
                    
                    Log.Information(string.Format(logInfo, led, _heartbeat));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(led), led, null);
            }
        }
    }
}

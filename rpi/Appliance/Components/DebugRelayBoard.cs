using System;

namespace Appliance.Components
{
    public class DebugRelayBoard : IRelayBoard
    {
        private bool _lightsGarden;
        private bool _alarmStrobe;
        private bool _alarmSiren;
        private bool _garageRemoteButton;

        public DebugRelayBoard()
        {
            _lightsGarden = false;
            _alarmStrobe = false;
            _alarmSiren = false;
            _garageRemoteButton = false;
        }

        public bool IsOn(Enums.Relay relay)
        {
            switch (relay)
            {
                case Enums.Relay.LightsGarden:
                    return _lightsGarden;
                case Enums.Relay.AlarmStrobe:
                    return _alarmStrobe;
                case Enums.Relay.AlarmSiren:
                    return _alarmSiren;
                case Enums.Relay.GarageRemoteButton:
                    return _garageRemoteButton;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relay), relay, null);
            }
        }

        public void Off(Enums.Relay relay, bool updateReportedProperties = true)
        {
            switch (relay)
            {
                case Enums.Relay.LightsGarden:
                    _lightsGarden = false;
                    break;
                case Enums.Relay.AlarmStrobe:
                    _alarmStrobe = false;
                    break;
                case Enums.Relay.AlarmSiren:
                    _alarmSiren = false;
                    break;
                case Enums.Relay.GarageRemoteButton:
                    _garageRemoteButton = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relay), relay, null);
            }
        }

        public void On(Enums.Relay relay, bool updateReportedProperties = true)
        {
            switch (relay)
            {
                case Enums.Relay.LightsGarden:
                    _lightsGarden = true;
                    break;
                case Enums.Relay.AlarmStrobe:
                    _alarmStrobe = true;
                    break;
                case Enums.Relay.AlarmSiren:
                    _alarmSiren = true;
                    break;
                case Enums.Relay.GarageRemoteButton:
                    _garageRemoteButton = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relay), relay, null);
            }
        }

        public IRelayBoard Relay(Enums.Relay relay)
        {
            throw new NotImplementedException();
        }

        public IRelayBoard On()
        {
            throw new NotImplementedException();
        }

        public IRelayBoard Off()
        {
            throw new NotImplementedException();
        }
    }
}

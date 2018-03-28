using System;
using System.Threading.Tasks;
using Appliance.Commands;
using Appliance.Helpers;
using MediatR;

namespace Appliance.Components
{
    /// <summary>
    /// This is our physical board of relays
    /// There are 2 ways to interact with relays:
    /// </summary>
    /// <example> 
    /// The first way is when interacting with a single relay. 
    /// This sends an <see cref="UpdateReportedPropertyCommand"/> per relay off/on interaction.
    /// <code>
    /// _relayBoard.On(Enums.Relay.AlarmStrobe);
    /// </code>
    /// </example>
    /// <example> 
    /// The second way is when interacting with multiple relays by chaining them together.
    /// This has the benefit of only sending a single <see cref="UpdateReportedPropertyCommand"/> per multiple relay off/on interactions.
    /// <code>
    /// _relayBoard.Relay(Enums.Relay.AlarmStrobe).Relay(Enums.Relay.AlarmSiren).On();
    /// </code>
    /// </example>
    public class RelayBoard : IRelayBoard
    {
        private readonly IMediator _mediator;
        private readonly Relay _lightsGarden;
        private readonly Relay _alarmStrobe;
        private readonly Relay _alarmSiren;
        private readonly Relay _garageRemoteButton;
        private readonly ConcurrentHashSet<Enums.Relay> _relayCollection = new ConcurrentHashSet<Enums.Relay>();

        public RelayBoard(IMediator mediator)
        {
            _mediator = mediator;
            _lightsGarden = new Relay(Enums.Relay.LightsGarden);
            _alarmStrobe = new Relay(Enums.Relay.AlarmStrobe);
            _alarmSiren = new Relay(Enums.Relay.AlarmSiren);
            _garageRemoteButton = new Relay(Enums.Relay.GarageRemoteButton);
        }

        public bool IsOn(Enums.Relay relay)
        {
            switch (relay)
            {
                case Enums.Relay.LightsGarden:
                    return _lightsGarden.State;
                case Enums.Relay.AlarmStrobe:
                    return _alarmStrobe.State;
                case Enums.Relay.AlarmSiren:
                    return _alarmSiren.State;
                case Enums.Relay.GarageRemoteButton:
                    return _garageRemoteButton.State;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relay), relay, null);
            }
        }

        public void Off(Enums.Relay relay, bool updateReportedProperties = true)
        {
            switch (relay)
            {
                case Enums.Relay.LightsGarden:
                    _lightsGarden.Off();
                    break;
                case Enums.Relay.AlarmStrobe:
                    _alarmStrobe.Off();
                    break;
                case Enums.Relay.AlarmSiren:
                    _alarmSiren.Off();
                    break;
                case Enums.Relay.GarageRemoteButton:
                    _garageRemoteButton.Off();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relay), relay, null);
            }

            if (updateReportedProperties)
                UpdateReportedProperties(relay);
        }

        public void On(Enums.Relay relay, bool updateReportedProperties = true)
        {
            switch (relay)
            {
                case Enums.Relay.LightsGarden:
                    _lightsGarden.On();
                    break;
                case Enums.Relay.AlarmStrobe:
                    _alarmStrobe.On();
                    break;
                case Enums.Relay.AlarmSiren:
                    _alarmSiren.On();
                    break;
                case Enums.Relay.GarageRemoteButton:
                    _garageRemoteButton.On();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(relay), relay, null);
            }

            if (updateReportedProperties)
                UpdateReportedProperties(relay);
        }

        public IRelayBoard Relay(Enums.Relay relay)
        {
            _relayCollection.Add(relay);
            return this;
        }

        public IRelayBoard On()
        {
            foreach (var relay in _relayCollection)
                On(relay, false);

            _relayCollection.Clear();

            UpdateReportedProperties();

            return this;
        }

        public IRelayBoard Off()
        {
            foreach (var relay in _relayCollection)
                Off(relay, false);

            _relayCollection.Clear();

            UpdateReportedProperties();

            return this;
        }

        private void UpdateReportedProperties(Enums.Relay relay)
        {
            if (ShouldIgnoreReportingPropertiesWhen(relay)) return;

            UpdateReportedProperties();
        }

        private void UpdateReportedProperties()
        {
            Task.Run(async () => await _mediator.Send(new UpdateReportedPropertyCommand(nameof(Config.RelayState), Config.RelayState)));
        }

        /// <summary>
        /// Ignore sending DeviceTwin reported state for these relays
        /// Garage remote button relay is just a momentary action that doesn't need to have a reported state
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        private static bool ShouldIgnoreReportingPropertiesWhen(Enums.Relay relay)
        {
            switch (relay)
            {
                case Enums.Relay.GarageRemoteButton:
                    return true;
                default:
                    return false;
            }
        }
    }
}

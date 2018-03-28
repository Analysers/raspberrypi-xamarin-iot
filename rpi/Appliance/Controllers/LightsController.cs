using System.Threading.Tasks;
using Appliance.Commands;
using Appliance.Components;
using Appliance.Events;
using MediatR;

namespace Appliance.Controllers
{
    /// <summary>
    /// Control light interactions
    /// </summary>
    public class LightsController : ILightsController
    {
        private readonly IMediator _mediator;
        private readonly IRelayBoard _relayBoard;
        private readonly ITimerEvents _timerEvents;

        public LightsController(IMediator mediator, IRelayBoard relayBoard, ITimerEvents timerEvents)
        {
            _mediator = mediator;
            _relayBoard = relayBoard;
            _timerEvents = timerEvents;
        }

        public void OnSunset()
        {
            _relayBoard.On(Enums.Relay.LightsGarden);

            var lightsOffEvent = _timerEvents.GetEvent(Enums.TimedEvent.OnLightsOff);
            lightsOffEvent.Enable();

            _timerEvents.UpdateEvent(Enums.TimedEvent.OnLightsOff, lightsOffEvent);
        }

        public void OnSunrise()
        {
            _relayBoard
                .Relay(Enums.Relay.LightsGarden)
                .Off();
        }

        public async Task LightsOff()
        {
            _relayBoard
                .Relay(Enums.Relay.LightsGarden)
                .Off();

            var evnt = _timerEvents.DisableEvent(Enums.TimedEvent.OnLightsOff);

            await _mediator.Send(new UpdateReportedPropertyCommand(nameof(Enums.TimedEvent.OnLightsOff), evnt));
        }

        public bool GardenLightsToggle()
        {
            return Toggle(Enums.Relay.LightsGarden);
        }

        private bool Toggle(Enums.Relay relay)
        {
            if (_relayBoard.IsOn(relay))
                _relayBoard.Off(relay);
            else
                _relayBoard.On(relay);

            return _relayBoard.IsOn(relay);
        }
    }
}

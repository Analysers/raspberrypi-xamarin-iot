using System.Threading.Tasks;
using Appliance.Components;

namespace Appliance.Controllers
{
    /// <summary>
    /// Control relay interactions
    /// </summary>
    public class RelaysController : IRelaysController
    {
        private readonly IRelayBoard _relayBoard;

        public RelaysController(IRelayBoard relayBoard)
        {
            _relayBoard = relayBoard;
        }

        public bool AlarmStrobeToggle()
        {
            return Toggle(Enums.Relay.AlarmStrobe);
        }

        public bool AlarmSirenToggle()
        {
            return Toggle(Enums.Relay.AlarmSiren);
        }

        public async Task PressGarageDoorRemoteButton()
        {
            _relayBoard.On(Enums.Relay.GarageRemoteButton);
            await Task.Delay(Config.MomentaryButtonPressMilliseconds);
            _relayBoard.Off(Enums.Relay.GarageRemoteButton);
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

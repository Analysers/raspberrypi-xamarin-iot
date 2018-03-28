using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Appliance.Components
{
    public class Relay
    {
        private readonly GpioPin _gpioPin;
        private readonly Enums.Relay _relay;

        public Relay(Enums.Relay relay)
        {
            _relay = relay;
            _gpioPin = Pi.Gpio[(int)relay];

            Initialize();
        }

        /// <summary>
        /// Relay is an output device
        /// We initialize or relays to LOW so they are reset when appliance is starting
        /// </summary>
        private void Initialize()
        {
            _gpioPin.PinMode = GpioPinDriveMode.Output;
            _gpioPin.Write(GpioPinValue.Low);
        }

        public bool State => _gpioPin.Read();

        public void Off()
        {
            _gpioPin.Write(GpioPinValue.Low);
            UpdateStateTo(false);
        }

        public void On()
        {
            _gpioPin.Write(GpioPinValue.High);
            UpdateStateTo(true);
        }

        private void UpdateStateTo(bool state)
        {
            switch (_relay)
            {
                case Enums.Relay.LightsGarden:
                    Config.RelayState.LightsGarden = state;
                    break;
                case Enums.Relay.AlarmStrobe:
                    Config.RelayState.AlarmStrobe = state;
                    break;
                case Enums.Relay.AlarmSiren:
                    Config.RelayState.AlarmSiren = state;
                    break;
                case Enums.Relay.GarageRemoteButton:
                    Config.RelayState.GarageRemoteButton = state;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

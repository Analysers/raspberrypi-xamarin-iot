using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Appliance.Components
{
    public class Led
    {
        private readonly GpioPin _gpioPin;

        public Led(Enums.Led led)
        {
            _gpioPin = Pi.Gpio[(int)led];

            Initialize();
        }

        /// <summary>
        /// LED is an output device, initialized off (LOW)
        /// </summary>
        private void Initialize()
        {
            _gpioPin.PinMode = GpioPinDriveMode.Output;
            _gpioPin.Write(GpioPinValue.Low);
        }

        public bool State
        {
            get => _gpioPin.Read();
            set => _gpioPin.Write(value);
        }
    }
}

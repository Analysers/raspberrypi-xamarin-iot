using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
using Unosquare.RaspberryIO.Native;

namespace Appliance.Components
{
    /// <summary>
    /// Momentary Push Button
    /// Some momentary push buttons can be sensitive to touch, so we only detect falling edge
    /// and throw away any interactions for the preceding milliseconds configured in <see cref="Config.MomentaryButtonPressMilliseconds"/>
    /// </summary>
    public class MomentaryButton
    {
        private readonly object _lock = new object();
        private readonly GpioPin _gpioPin;
        public event EventHandler<EventArgs> Pressed;
        private ulong _lastInterruptTime;

        public MomentaryButton(Enums.Button button)
        {
            _gpioPin = Pi.Gpio[(int)button];

            Initialize();
        }

        /// <summary>
        /// Momentary Push Button is an input device
        /// We have a pull resister on our physical board
        /// Our physical board connection has a digital read of HIGH on an open circuit, so on
        /// button press we detect on failling edge (to LOW)
        /// </summary>
        private void Initialize()
        {
            _gpioPin.PinMode = GpioPinDriveMode.Input;
            _gpioPin.InputPullMode = GpioPinResistorPullMode.Off;
            _gpioPin.RegisterInterruptCallback(EdgeDetection.FallingEdge, OnPress);
        }

        private void OnPress()
        {
            lock (_lock)
            {
                if (_gpioPin.Read()) return;

                ulong interruptTime = WiringPi.Millis();
                if (interruptTime - _lastInterruptTime <= Convert.ToUInt64(Config.MomentaryButtonPressMilliseconds.Milliseconds)) return;

                _lastInterruptTime = interruptTime;
                Pressed?.Invoke(this, new EventArgs());
            }
        }

    }
}

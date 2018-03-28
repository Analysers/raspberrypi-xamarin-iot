using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Appliance.Components
{
    public class Sensor
    {
        private readonly object _lock = new object();
        private readonly GpioPin _gpioPin;
        public event EventHandler<EventArgs> TrippedChanged;
        private bool _value;
        public Enums.Sensor Type { get; }

        public Sensor(Enums.Sensor sensor)
        {
            _gpioPin = Pi.Gpio[(int)sensor];
            Type = sensor;

            Initialize();
        }

        /// <summary>
        /// A sensor is an input device
        /// We have a pull resister on our physical board per sensor
        /// Our physical board connection has a digital read of HIGH on an open circuit,
        /// we detect both rising and failling edges for our sensors
        /// </summary>
        private void Initialize()
        {
            _gpioPin.PinMode = GpioPinDriveMode.Input;
            _gpioPin.InputPullMode = GpioPinResistorPullMode.Off;
            _gpioPin.RegisterInterruptCallback(EdgeDetection.RisingAndFallingEdges, OnValueChanged);
            _value = Tripped;
        }

        public bool Tripped => _gpioPin.Read();

        private void OnValueChanged()
        {
            lock (_lock)
            {
                var result = Tripped;
                if (_value == result) return;

                _value = result;
                TrippedChanged?.Invoke(this, new EventArgs());
            }
        }
    }
}

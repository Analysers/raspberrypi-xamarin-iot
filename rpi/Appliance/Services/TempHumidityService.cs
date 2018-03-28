using Serilog;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Peripherals;

namespace Appliance.Services
{
    public class TempHumidityService : ITempHumidityService
    {
        private readonly TemperatureSensorAM2302 _tempSensor;

        public TempHumidityService()
        {
            _tempSensor = new TemperatureSensorAM2302(Pi.Gpio[5]);
        }

        public void Start()
        {
            _tempSensor.Start();

            _tempSensor.OnDataAvailable += (sender, eventArgs) =>
            {
                if (eventArgs.TemperatureCelsius == Config.Temperature &&
                    eventArgs.HumidityPercentage == Config.Humidity) return;

                Config.Temperature = eventArgs.TemperatureCelsius;
                Config.Humidity = eventArgs.HumidityPercentage;

                Log.Information($"{Config.Temperature} °C");
                Log.Information($"{Config.Humidity}% Humidity");
            };
        }
    }
}

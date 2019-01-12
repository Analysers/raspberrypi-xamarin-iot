using Appliance.Azure;
using Appliance.Components;
using Appliance.Domain;
using Appliance.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.Runtime.Loader;
using System.Threading.Tasks;
using static System.Threading.Thread;
using static System.Threading.Timeout;

namespace Appliance
{
    internal class Program
    {
        private static IServiceProvider _serviceProvider;
        private static IMediator _mediator;
        private static IAzureIoTHub _azureIoTHub;

        private static async Task Main(string[] args)
        {
            ConfigureLogger();

            _serviceProvider = ServiceCollectionFactory.CreateServiceProvider();
            _mediator = _serviceProvider.GetService<IMediator>();

            AppDomain.CurrentDomain.ProcessExit += (s, ev) =>
            {
                Log.Information("Process exit...");
            };

            AssemblyLoadContext.Default.Unloading += context =>
            {
                _azureIoTHub.TryClose();
                Log.Information("Unloading...");
            };

            await InitializeAppliance();

            Log.Information("Home Security System started");

            // Sleep indefinitely
            Sleep(Infinite);
        }

        private static async Task InitializeAppliance()
        {
            // Initialize Config
            Config.Initialize(_serviceProvider.GetService<IArmedState>());

            // Initialize Azure IoT Hub
            _azureIoTHub = _serviceProvider.GetService<IAzureIoTHub>();
            await _azureIoTHub.Initialize();

            // Initialize Sensors
            var sensorController = _serviceProvider.GetService<ISensorBoard>();
            await sensorController.Initialize();

            // Initialize Temperature sensor
            var networkCabinetTempService = _serviceProvider.GetService<ITempHumidityService>();
            networkCabinetTempService.Start();

            // Start Timer
            var timer = _serviceProvider.GetService<ITimerService>();
            timer.Start();
            
            // Update sunrise/sunset times for garden lights
            //await _mediator.Publish(new UpdateSunriseSunsetCommand()); // Uncomment if you require OpenWeather data, and add App ID in Config.cs

            Log.Information("Initialization complete");
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}

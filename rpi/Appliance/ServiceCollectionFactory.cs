using Appliance.Azure;
using Appliance.Components;
using Appliance.Controllers;
using Appliance.Domain;
using Appliance.Events;
using Appliance.Services;
using Easy.Common;
using Easy.Common.Extensions;
using Easy.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Appliance
{
    public static class ServiceCollectionFactory
    {
        public static IServiceProvider CreateServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddMediator()
                .AddSingleton<IArmedState, ArmedState>()
                .AddSingleton<IAlarmState, AlarmState>()
                .AddSingleton<ILightsController, LightsController>()
                .AddSingleton<IRelaysController, RelaysController>()
                .AddSingleton<IGarageController, GarageController>()
                .AddSingleton<IOpenWeatherService, OpenWeatherService>()
                .AddSingleton<IRingService, RingService>()
                .AddSingleton<ITimerEvents, TimerEvents>()
                .AddSingleton<ITimerService, TimerService>()
                .AddSingleton<IClock, Clock>()
                .AddSingleton<IRestClient>(new RestClient(new Dictionary<string, string>
                {
                    {"Accept", "application/json"},
                    {"AcceptEncoding", "gzip"},
                }))
                .AddSingleton<ITimerClock>(new TimerClock(1.Seconds())); // Raise the Easy.Common.TimerClock.Tick event every second for the TimerService

#if DEBUG
            serviceProvider
                //.AddSingleton<IAzureIoTHub, DebugAzureIoTHub>()
                .AddSingleton<IAzureIoTHub, AzureIoTHub>()
                .AddSingleton<ILedBoard, DebugLedBoard>()
                .AddSingleton<IRelayBoard, DebugRelayBoard>()
                .AddSingleton<ISensorBoard, DebugSensorBoard>()
                .AddSingleton<ITempHumidityService, DebugTempHumidityService>();
#else
            serviceProvider
                .AddSingleton<IAzureIoTHub, AzureIoTHub>()
                .AddSingleton<ILedBoard, LedBoard>()
                .AddSingleton<IRelayBoard, RelayBoard>()
                .AddSingleton<ISensorBoard, SensorBoard>()
                .AddSingleton<ITempHumidityService, TempHumidityService>();
#endif

            return serviceProvider.BuildServiceProvider();
        }

        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(typeof(Program));
            return services;
        }
    }
}

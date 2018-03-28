using System;
using System.Text;
using System.Threading.Tasks;
using Appliance.Commands;
using Appliance.Domain;
using Appliance.Helpers;
using Appliance.Notifications;
using MediatR;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Serilog;
using JsonSerializer = Utf8Json.JsonSerializer;

namespace Appliance.Azure
{
    public class AzureIoTHub : IAzureIoTHub
    {
        private readonly IMediator _mediator;
        private readonly IAlarmState _alarmState;
        private readonly DeviceClient _deviceClient;

        public AzureIoTHub(IMediator mediator, IAlarmState alarmState, DeviceClient deviceClient)
        {
            _mediator = mediator;
            _alarmState = alarmState;
            _deviceClient = deviceClient;
        }

        public async Task Initialize()
        {
            await RegisterTwinUpdateAsync();
            var tc = await GetDesiredProperties();
            await UpdateDeviceTwin(tc);
            await RegisterDirectMethodHandlers();
            // Receive Cloud to Device messages init can go here if required
        }

        private async Task RegisterDirectMethodHandlers()
        {
            await _deviceClient.SetMethodHandlerAsync(nameof(ToggleGardenLights), ToggleGardenLights, null);
            await _deviceClient.SetMethodHandlerAsync(nameof(ToggleAlarmStrobe), ToggleAlarmStrobe, null);
            await _deviceClient.SetMethodHandlerAsync(nameof(ToggleAlarmSiren), ToggleAlarmSiren, null);
            await _deviceClient.SetMethodHandlerAsync(nameof(PressGarageDoorButton), PressGarageDoorButton, null);
            await _deviceClient.SetMethodHandlerAsync(nameof(WakePc), WakePc, null);
            await _deviceClient.SetMethodHandlerAsync(nameof(RegisterAppForPushNotifications), RegisterAppForPushNotifications, null);
        }

        private async Task<MethodResponse> ToggleGardenLights(MethodRequest methodRequest, object userContext)
        {
            var state = await _mediator.Send(new ToggleGardenLightsCommand());
            return new MethodResponse(JsonSerializer.Serialize(state), 200);
        }

        private async Task<MethodResponse> ToggleAlarmStrobe(MethodRequest methodRequest, object userContext)
        {
            var state = await _mediator.Send(new ToggleAlarmStrobeCommand());
            return new MethodResponse(JsonSerializer.Serialize(state), 200);
        }

        private async Task<MethodResponse> ToggleAlarmSiren(MethodRequest methodRequest, object userContext)
        {
            var state = await _mediator.Send(new ToggleAlarmSirenCommand());
            return new MethodResponse(JsonSerializer.Serialize(state), 200);
        }

        private async Task<MethodResponse> PressGarageDoorButton(MethodRequest methodRequest, object userContext)
        {
            await _mediator.Publish(new PressGarageDoorRemoteButtonCommand());
            return new MethodResponse(200);
        }

        private async Task<MethodResponse> WakePc(MethodRequest methodRequest, object userContext)
        {
            await _mediator.Send(new WakePcCommand());
            return new MethodResponse(200);
        }

        private async Task<MethodResponse> RegisterAppForPushNotifications(MethodRequest methodRequest, object userContext)
        {
            try
            {
                var command = JsonConvert.DeserializeObject<RegisterAppForPushNotificationsCommand>(methodRequest.DataAsJson);
                await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error registering device for push notifications");
            }

            return new MethodResponse(Encoding.UTF8.GetBytes(""), 200);
        }

        private async Task<TwinCollection> GetDesiredProperties()
        {
            Log.Information("Getting desired device twin properties");

            try
            {
                var twin = await _deviceClient.GetTwinAsync();
                return twin.Properties.Desired;
            }
            catch (Exception ex)
            {
                Log.Error("Error occured on GetDesiredProperties:", ex);
            }

            return new TwinCollection();
        }

        private async Task DesiredPropertiesUpdated(TwinCollection desiredProperties, object userContext)
        {
            await UpdateDeviceTwin(desiredProperties);
        }

        private async Task RegisterTwinUpdateAsync()
        {
            Log.Information("Registering Device Twin update callback");
            await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(DesiredPropertiesUpdated, null);
        }

        private async Task UpdateDeviceTwin(TwinCollection desiredProperties)
        {
            Log.Information("Desired properties updating...");

            try
            {
                var reportedProperties = UpdateStates(desiredProperties);
                await _deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
                
                await _mediator.Send(new PushNotificationCommand());
            }
            catch (Exception ex)
            {
                Log.Error("Error occured on UpdateDeviceTwin:", ex);
            }
        }

        public async Task UpdateReportedProperties(TwinCollection reportedProperties)
        {
            Log.Information("Updating reported properties...");

            try
            {
                await _deviceClient.UpdateReportedPropertiesAsync(reportedProperties);

                await _mediator.Send(new PushNotificationCommand());
            }
            catch (Exception ex)
            {
                Log.Error("Error occured on UpdateReportedProperties:", ex);
            }
        }

        public async Task UpdateReportedProperty(string key, object value)
        {
            Log.Information($"Updating reported property for key {key}");

            try
            {
                var reportedProperty = new TwinCollection { [key] = value };
                await _deviceClient.UpdateReportedPropertiesAsync(reportedProperty);

                await _mediator.Send(new PushNotificationCommand());
            }
            catch (Exception ex)
            {
                Log.Error("Error occured on UpdateReportedProperties:", ex);
            }
        }

        private TwinCollection UpdateStates(TwinCollection desiredProperties)
        {
            var reportedProperties = new TwinCollection();

            // Occupant State
            UpdateOccupantStateReportedProperties(desiredProperties, reportedProperties);

            // Armed State
            reportedProperties[nameof(ArmedState)] = Config.ArmedState;

            return reportedProperties;
        }

        private void UpdateOccupantStateReportedProperties(TwinCollection desiredProperties,
            TwinCollection reportedProperties)
        {
            const string occupantState = nameof(Config.Occupant);
            try
            {
                if (desiredProperties.Contains(occupantState))
                {
                    OccupantState newOccupantState = JsonConvert.DeserializeObject<OccupantState>(desiredProperties[occupantState].ToString());
                    _alarmState.OccupantStateChanged(Config.Occupant, newOccupantState);

                    Config.Occupant = newOccupantState;
                    reportedProperties[occupantState] = Config.Occupant;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error on UpdateState '{occupantState}'", ex);
            }
        }
    }
}
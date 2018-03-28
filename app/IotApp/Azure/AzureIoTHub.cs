using System;
using System.Threading.Tasks;
using IotApp.Helpers;
using IotApp.Models;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace IotApp.Azure
{
    public class AzureIoTHub : IAzureIoTHub
    {
        public static readonly AsyncLock Lock = new AsyncLock();
        private readonly IDeviceTwin _deviceTwin;
        private readonly ISettings _settings;
        private static RegistryManager _registryManager;
        private static ServiceClient _serviceClient;

        private const string DeviceId = "device-id";
        private const string ConnectionString = "azure-iot-hub-shared-access-connection-string";

        public AzureIoTHub(IDeviceTwin deviceTwin, ISettings settings)
        {
            _deviceTwin = deviceTwin;
            _settings = settings;
            _registryManager = RegistryManager.CreateFromConnectionString(ConnectionString);
            _serviceClient = ServiceClient.CreateFromConnectionString(ConnectionString);
        }

        public async Task GetDeviceTwin()
        {
            var twin = await _registryManager.GetTwinAsync(DeviceId);
            AppCenterHelper.Track(nameof(GetDeviceTwin));

            // Update DeviceTwin
            _deviceTwin.ApplianceConnected = twin.ConnectionState == DeviceConnectionState.Connected;

            try
            {
                var reportedPropertyForRelayState = twin.Properties.Reported["RelayState"];
                RelayState relayState = JsonConvert.DeserializeObject<RelayState>(reportedPropertyForRelayState.ToJson());
                _deviceTwin.LightsGarden = relayState.LightsGarden;
                _deviceTwin.AlarmStrobe = relayState.AlarmStrobe;
                _deviceTwin.AlarmSiren = relayState.AlarmSiren;
            }
            catch (Exception ex)
            {
                AppCenterHelper.Error("Error updating RelayState", ex);
            }

            try
            {
                var reportedPropertyOccupantState = twin.Properties.Reported[nameof(_deviceTwin.Occupant)];
                var occupant = JsonConvert.DeserializeObject<OccupantState>(reportedPropertyOccupantState.ToJson());
                _deviceTwin.Occupant = occupant;
            }
            catch (Exception ex)
            {
                AppCenterHelper.Error("Error updating Occupant", ex);
            }

            try
            {
                var reportedPropertyArmedState = twin.Properties.Reported[nameof(ArmedState)];
                var armedState = JsonConvert.DeserializeObject<ArmedState>(reportedPropertyArmedState.ToJson());
                _deviceTwin.ArmedState = armedState;
            }
            catch (Exception ex)
            {
                AppCenterHelper.Error("Error updating ArmedState", ex);
            }

            try
            {
                _deviceTwin.GarageDoorOpen = (bool)twin.Properties.Reported["GarageDoorOpen"];
            }
            catch (Exception ex)
            {
                AppCenterHelper.Error("Error updating GarageDoorOpen", ex);
            }
        }

        public async Task UpdateOccupantState(OccupantState occupantState)
        {
            using (await Lock.LockAsync())
            {
                switch (Config.Person)
                {
                    case "Occupant":
                        if (_deviceTwin.Occupant.GetHashCode() == occupantState.GetHashCode())
                        {
                            AppCenterHelper.Track("UpdateOccupantState passed");
                            return;
                        }

                        break;
                }

                dynamic desiredState;
                switch (Config.Person)
                {
                    case "Occupant":
                    default:
                        desiredState = new
                        {
                            Occupant = occupantState
                        };
                        break;
                }

                var patch = new
                {
                    properties = new
                    {
                        desired = desiredState
                    }
                };


                // TODO: This can fail, add in resiliency
                var updatedTwin =
                    await _registryManager.UpdateTwinAsync(DeviceId, JsonConvert.SerializeObject(patch), "*");

                await GetDeviceTwin();

                AppCenterHelper.Track("Updated Occupant State");
            }
        }

        public async Task<(int Status, string ResponsePayload)> InvokeMethod(string methodName, string reqPayload = "{}")
        {
            var methodInvocation =
                new CloudToDeviceMethod(methodName)
                {
                    ResponseTimeout = TimeSpan.FromSeconds(10),
                    ConnectionTimeout = TimeSpan.FromSeconds(10)
                };
            methodInvocation.SetPayloadJson(reqPayload);

            var response = await _serviceClient.InvokeDeviceMethodAsync(DeviceId, methodInvocation);

            return (response.Status, response.GetPayloadAsJson());
        }

        public async Task<string> QueryDeviceTwin()
        {
            var twin = await _registryManager.GetTwinAsync(DeviceId);

            if (twin == null)
                return "Error querying device twin";

            return JsonConvert.SerializeObject(twin, Formatting.Indented);
        }
    }
}

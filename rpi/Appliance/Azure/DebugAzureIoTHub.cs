using Microsoft.Azure.Devices.Shared;
using Serilog;
using System.Threading.Tasks;

namespace Appliance.Azure
{
    public class DebugAzureIoTHub : IAzureIoTHub
    {
        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public Task TryClose()
        {
            return Task.CompletedTask;
        }

        public Task UpdateReportedProperties(TwinCollection reportedProperties)
        {
            Log.Information("DebugAzureIoTHub UpdateReportedProperties called");
            return Task.CompletedTask;
        }

        public Task UpdateReportedProperty(string key, object value)
        {
            Log.Information("DebugAzureIoTHub UpdateReportedProperty called");
            return Task.CompletedTask;
        }
    }
}

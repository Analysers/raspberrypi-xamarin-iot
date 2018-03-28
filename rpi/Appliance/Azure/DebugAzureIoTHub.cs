using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using Serilog;

namespace Appliance.Azure
{
    public class DebugAzureIoTHub : IAzureIoTHub
    {
        public Task Initialize()
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

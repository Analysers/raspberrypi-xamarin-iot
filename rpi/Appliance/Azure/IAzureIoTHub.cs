using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;

namespace Appliance.Azure
{
    public interface IAzureIoTHub
    {
        Task Initialize();
        Task UpdateReportedProperties(TwinCollection reportedProperties);
        Task UpdateReportedProperty(string key, object value);
    }
}
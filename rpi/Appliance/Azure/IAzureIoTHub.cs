using Microsoft.Azure.Devices.Shared;
using System.Threading.Tasks;

namespace Appliance.Azure
{
    public interface IAzureIoTHub
    {
        Task Initialize();
        Task TryClose();
        Task UpdateReportedProperties(TwinCollection reportedProperties);
        Task UpdateReportedProperty(string key, object value);
    }
}
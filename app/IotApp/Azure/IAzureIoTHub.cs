using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using IotApp.Models;

namespace IotApp.Azure
{
    public interface IAzureIoTHub
    {
        Task GetDeviceTwin();
        Task UpdateOccupantState(OccupantState occupantState);
        Task<(int Status, string ResponsePayload)> InvokeMethod(string methodName, string reqPayload = "{}");
        Task<string> QueryDeviceTwin();
    }
}
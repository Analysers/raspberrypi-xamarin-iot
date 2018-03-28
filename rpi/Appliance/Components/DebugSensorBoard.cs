using System.Threading.Tasks;
using Serilog;

namespace Appliance.Components
{
    public class DebugSensorBoard : ISensorBoard
    {
        public DebugSensorBoard()
        {
            Log.Information("Sensors started");
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public bool GarageDoorOpen()
        {
            return false;
        }
    }
}

using System.Threading.Tasks;
using Appliance.Enums;

namespace Appliance.Domain
{
    public interface IAlarmState
    {
        void Reset(Sensor sensor, bool enableAlarmActivatedLed = true);
        Task Tripped(Sensor sensor);
        bool SensorArmed(Sensor sensor);
        void OccupantStateChanged(OccupantState michaelState, OccupantState newOccupantState);
        void Disarm();
    }
}
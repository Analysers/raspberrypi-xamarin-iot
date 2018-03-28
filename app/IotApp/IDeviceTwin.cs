using System.ComponentModel;
using IotApp.Models;

namespace IotApp
{
    public interface IDeviceTwin
    {
        bool ApplianceConnected { get; set; }
        bool LightsGarden { get; set; }
        bool AlarmStrobe { get; set; }
        bool AlarmSiren { get; set; }
        OccupantState Occupant { get; set; }
        ArmedState ArmedState { get; set; }
        bool GarageDoorOpen { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}
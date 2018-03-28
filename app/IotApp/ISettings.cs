using System.ComponentModel;
using IotApp.Models;

namespace IotApp
{
    public interface ISettings
    {
        OccupantState OccupantState { get; set; }
        ArmedState ArmedState { get; set; }
        bool AtHome { get; set; }
        bool AtBedroom { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}
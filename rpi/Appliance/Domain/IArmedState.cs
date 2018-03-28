namespace Appliance.Domain
{
    public interface IArmedState
    {
        bool ArmedAwayDay { get; }
        bool ArmedAwayNight { get; }
        bool ArmedSleeping { get; }
        bool Disarmed { get; }
        bool GarageDoorArmed { get; }
        bool FrontDoorArmed { get; }
    }
}
namespace Appliance.Components
{
    public interface IRelayBoard
    {
        bool IsOn(Enums.Relay relay);
        void Off(Enums.Relay relay, bool updateReportedProperties = true);
        void On(Enums.Relay relay, bool updateReportedProperties = true);
        IRelayBoard Relay(Enums.Relay relay);
        IRelayBoard On();
        IRelayBoard Off();
    }
}
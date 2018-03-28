namespace Appliance.Components
{
    public interface ILedBoard
    {
        bool IsOn(Enums.Led led);
        void Off(Enums.Led led);
        void On(Enums.Led led);
        void Toggle(Enums.Led led);
    }
}
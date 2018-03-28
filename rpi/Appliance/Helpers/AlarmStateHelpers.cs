using Appliance.Enums;

namespace Appliance.Helpers
{
    public static class AlarmStateHelpers
    {
        public static bool IsSensorTamper(this Sensor sensor)
        {
            return sensor == Sensor.OutdoorTamper;
        }

        public static bool IsSensorOutdoor(this Sensor sensor)
        {
            return sensor == Sensor.OutdoorMotion;
        }

        public static bool IsFrontDoor(this Sensor sensor)
        {
            return sensor == Sensor.FrontDoorMotion;
        }

        public static bool IsGarageDoor(this Sensor sensor)
        {
            return sensor == Sensor.GarageDoor;
        }
    }
}

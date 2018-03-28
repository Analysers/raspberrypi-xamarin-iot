namespace IotApp.Models
{
    public class ArmedState
    {
        public bool ArmedAwayDay { get; set; }
        public bool ArmedAwayNight { get; set; }
        public bool ArmedSleeping { get; set; }
        public bool Disarmed { get; set; }
        public bool GarageDoorArmed { get; set; }
        public bool FrontDoorArmed { get; set; }
    }

}

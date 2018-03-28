using Easy.Common.Interfaces;
using Appliance.Helpers;

namespace Appliance.Domain
{
    public class ArmedState : IArmedState
    {
        private readonly IClock _clock;

        public ArmedState(IClock clock)
        {
            _clock = clock;
        }

        public bool Disarmed => !ArmedStay && !ArmedSleeping && !ArmedAwayNight && !ArmedAwayDay;

        public bool ArmedStay => Config.OccupantsHome && _clock.Now.Within(Config.OccupantsHomeArmTime, Config.OccupantsHomeDisarmTime);

        public bool ArmedSleeping
        {
            get
            {
                bool IsSleeping()
                {
                    return Config.OccupantAtBedroom;
                }

                return ArmedStay && IsSleeping();
            }
        }

        public bool ArmedAwayNight => !Config.OccupantsHome && _clock.Now.Within(Config.OccupantsHomeArmTime, Config.OccupantsHomeDisarmTime);

        public bool ArmedAwayDay => !Config.OccupantsHome && !_clock.Now.Within(Config.OccupantsHomeArmTime, Config.OccupantsHomeDisarmTime);

        public bool GarageDoorArmed => ArmedAwayNight || ArmedAwayDay || ArmedSleeping;

        public bool FrontDoorArmed => Config.ArmedState.ArmedAwayNight || Config.ArmedState.ArmedSleeping;
    }
}

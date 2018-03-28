using Newtonsoft.Json;

namespace Appliance.Domain
{

    public class OccupantState
    {
        public bool AtHome { get; }
        public bool IsSleeping { get; }

        public OccupantState()
        {
            AtHome = false;
            IsSleeping = false;
        }

        [JsonConstructor]
        public OccupantState(bool atHome, bool isSleeping)
        {
            AtHome = atHome;
            IsSleeping = isSleeping;
        }
    }
}

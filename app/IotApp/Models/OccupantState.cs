namespace IotApp.Models
{
    public class OccupantState
    {
        public bool AtHome { get; set; }
        public bool IsSleeping { get; set; }

        public override int GetHashCode()
        {
            var hash = 0;

            if (AtHome)
                hash |= 1 << 0;
            if (IsSleeping)
                hash |= 1 << 1;

            return hash;
        }
    }
}

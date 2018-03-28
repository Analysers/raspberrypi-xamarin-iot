namespace Appliance.Models
{
    public class StateResponse
    {
        public bool State { get; }

        public StateResponse(bool state)
        {
            State = state;
        }
    }
}

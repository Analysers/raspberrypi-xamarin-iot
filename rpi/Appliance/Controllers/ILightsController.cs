using System.Threading.Tasks;

namespace Appliance.Controllers
{
    public interface ILightsController
    {
        bool GardenLightsToggle();
        Task LightsOff();
        void OnSunrise();
        void OnSunset();
    }
}
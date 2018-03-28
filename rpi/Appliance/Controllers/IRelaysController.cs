using System.Threading.Tasks;

namespace Appliance.Controllers
{
    public interface IRelaysController
    {
        bool AlarmStrobeToggle();
        bool AlarmSirenToggle();
        Task PressGarageDoorRemoteButton();
    }
}
using System.Threading.Tasks;

namespace Appliance.Components
{
    public interface ISensorBoard
    {
        Task Initialize();
        bool GarageDoorOpen();
    }
}
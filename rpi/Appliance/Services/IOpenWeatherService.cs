using System.Threading;
using System.Threading.Tasks;

namespace Appliance.Services
{
    public interface IOpenWeatherService
    {
        Task UpdateWeatherData(CancellationToken cancellationToken);
    }
}
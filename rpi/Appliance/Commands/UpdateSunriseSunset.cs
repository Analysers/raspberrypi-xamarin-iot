using System.Threading;
using System.Threading.Tasks;
using Appliance.Services;
using MediatR;

namespace Appliance.Commands
{
    public class UpdateSunriseSunsetHandler : INotificationHandler<UpdateSunriseSunsetCommand>
    {
        private readonly IOpenWeatherService _openWeatherService;

        public UpdateSunriseSunsetHandler(IOpenWeatherService openWeatherService)
        {
            _openWeatherService = openWeatherService;
        }

        public Task Handle(UpdateSunriseSunsetCommand notification, CancellationToken cancellationToken)
        {
            return _openWeatherService.UpdateWeatherData(cancellationToken);
        }
    }

    public class UpdateSunriseSunsetCommand : INotification
    {
    }
}

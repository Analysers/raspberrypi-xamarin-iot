using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;

namespace Appliance.Commands
{
    public class RegisterAppForPushNotificationsHandler : IRequestHandler<RegisterAppForPushNotificationsCommand>
    {
        public Task Handle(RegisterAppForPushNotificationsCommand message, CancellationToken cancellationToken)
        {
            // TODO: Register app device for push notification using your preferred method
            Log.Information("Registered app device for push notifications");
            return Task.CompletedTask;
        }
    }

    public class RegisterAppForPushNotificationsCommand : IRequest
    {
        public string DeviceToken { get; set; }
        public string User { get; set; }
    }
}

using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Appliance.Commands
{
    public class RegisterAppForPushNotificationsHandler : IRequestHandler<RegisterAppForPushNotificationsCommand>
    {
        public Task<Unit> Handle(RegisterAppForPushNotificationsCommand message, CancellationToken cancellationToken)
        {
            // TODO: Register app device for push notification using your preferred method
            Log.Information("Registered app device for push notifications");
            return Unit.Task;
        }
    }

    public class RegisterAppForPushNotificationsCommand : IRequest
    {
        public string DeviceToken { get; set; }
        public string User { get; set; }
    }
}

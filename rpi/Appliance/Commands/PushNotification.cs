using Appliance.Models;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Appliance.Commands
{
    public class PushNotificationHandler : IRequestHandler<PushNotificationCommand>
    {
        Task<Unit> IRequestHandler<PushNotificationCommand, Unit>.Handle(PushNotificationCommand message, CancellationToken cancellationToken)
        {
            // TODO: Send push notification using your preferred method
            Log.Information("Push notification sent");
            return Unit.Task;
        }
    }

    public class PushNotificationCommand : IRequest
    {
        public PushNotificationPayload Payload { get; set; } = new PushNotificationPayload();
    }
}

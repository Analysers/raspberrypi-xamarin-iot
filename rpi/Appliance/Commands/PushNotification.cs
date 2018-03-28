using Appliance.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Appliance.Commands
{
    public class PushNotificationHandler : IRequestHandler<PushNotificationCommand>
    {
        public Task Handle(PushNotificationCommand message, CancellationToken cancellationToken)
        {
            // TODO: Send push notification using your preferred method
            Log.Information("Push notification sent");
            return Task.CompletedTask;
        }
    }

    public class PushNotificationCommand : IRequest
    {
        public PushNotificationPayload Payload { get; set; } = new PushNotificationPayload();
    }
}

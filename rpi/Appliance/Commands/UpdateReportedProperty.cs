using Appliance.Azure;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appliance.Commands
{
    public class UpdateReportedPropertyHandler : AsyncRequestHandler<UpdateReportedPropertyCommand>
    {
        private readonly IAzureIoTHub _azureIoTHub;

        public UpdateReportedPropertyHandler(IAzureIoTHub azureIoTHub)
        {
            _azureIoTHub = azureIoTHub;
        }

        protected override Task Handle(UpdateReportedPropertyCommand request, CancellationToken cancellationToken)
        {
            return _azureIoTHub.UpdateReportedProperty(request.Key, request.Value);
        }
    }

    public class UpdateReportedPropertyCommand : IRequest
    {
        public string Key { get; }
        public object Value { get; }

        public UpdateReportedPropertyCommand(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}

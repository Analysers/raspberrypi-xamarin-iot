using Appliance.Azure;
using MediatR;
using Microsoft.Azure.Devices.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace Appliance.Commands
{
    public class UpdateReportedPropertiesHandler : AsyncRequestHandler<UpdateReportedPropertiesCommand>
    {
        private readonly IAzureIoTHub _azureIoTHub;

        public UpdateReportedPropertiesHandler(IAzureIoTHub azureIoTHub)
        {
            _azureIoTHub = azureIoTHub;
        }

        protected override Task Handle(UpdateReportedPropertiesCommand request, CancellationToken cancellationToken)
        {
            return _azureIoTHub.UpdateReportedProperties(request.ReportedProperties);
        }
    }

    public class UpdateReportedPropertiesCommand : IRequest
    {
        public TwinCollection ReportedProperties { get; }

        public UpdateReportedPropertiesCommand(TwinCollection reportedProperties)
        {
            ReportedProperties = reportedProperties;
        }
    }
}

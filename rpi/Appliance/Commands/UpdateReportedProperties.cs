using MediatR;
using Microsoft.Azure.Devices.Shared;
using System.Threading.Tasks;
using Appliance.Azure;
using Appliance.Helpers;

namespace Appliance.Commands
{
    public class UpdateReportedPropertiesHandler : AsyncRequestHandler<UpdateReportedPropertiesCommand>
    {
        private readonly IAzureIoTHub _azureIoTHub;

        public UpdateReportedPropertiesHandler(IAzureIoTHub azureIoTHub)
        {
            _azureIoTHub = azureIoTHub;
        }

        protected override Task HandleCore(UpdateReportedPropertiesCommand request)
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

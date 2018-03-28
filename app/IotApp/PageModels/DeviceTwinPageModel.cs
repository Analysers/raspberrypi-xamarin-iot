using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FreshMvvm;
using IotApp.Azure;
using IotApp.Helpers;
using PropertyChanged;
using Xamarin.Forms;

namespace IotApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class DeviceTwinPageModel : FreshBasePageModel
    {
        private readonly Stopwatch _stopwatch;
        private readonly IAzureIoTHub _azureIoTHub;
        public string IoTHubOutput { get; set; } = "Loading...";

        public DeviceTwinPageModel(IAzureIoTHub azureIoTHub)
        {
            _azureIoTHub = azureIoTHub;
            _stopwatch = new Stopwatch();
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            AppCenterHelper.Track("DeviceTwinPageModel - ViewIsAppearing");
            await OutputDeviceTwin();

            base.ViewIsAppearing(sender, e);
        }

        public Command OnRefreshTapped
        {
            get => new Command(async () =>
            {
                AppCenterHelper.Track("DeviceTwinPageModel - OnRefreshTapped");
                await OutputDeviceTwin();
            });
        }

        private async Task OutputDeviceTwin()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            var resp = await _azureIoTHub.QueryDeviceTwin();

            _stopwatch.Stop();

            IoTHubOutput = String.Format("{0:d/M/yyyy HH:mm:ss}", DateTime.Now) + " in " + _stopwatch.ElapsedMilliseconds + "ms" + Environment.NewLine + resp;
        }
    }
}

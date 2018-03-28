using System;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using FreshMvvm;
using IotApp.Azure;
using IotApp.Helpers;
using IotApp.Models;
using Newtonsoft.Json;
using Polly;
using PropertyChanged;
using Xamarin.Forms;

namespace IotApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class SecurityPageModel : FreshBasePageModel
    {
        public string PanicButtonText { get; set; } = "Sound Panic Alarm";
        public string GarageOperatingText { get; set; } = "Operating";
        public bool IsGarageOperating { get; set; } = false;

        public IDeviceTwin DeviceTwin { get; set; }
        public ISettings Settings { get; }
        private readonly IAzureIoTHub _azureIoTHub;
        private readonly IUserDialogs _dialogs;

        public SecurityPageModel(IDeviceTwin deviceTwin, ISettings settings, IAzureIoTHub azureIoTHub, IUserDialogs dialogs)
        {
            DeviceTwin = deviceTwin;
            Settings = settings;
            _azureIoTHub = azureIoTHub;
            _dialogs = dialogs;
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            await _azureIoTHub.GetDeviceTwin();

            AppCenterHelper.Track("SecurityPageModel - ViewIsAppearing");

            base.ViewIsAppearing(sender, e);
        }

        private async Task<bool> InvokeMethodWithResponseState(string method, bool currentState)
        {
            AppCenterHelper.Track($"SecurityPageModel - {method}");

            var response = await ResilientCall.ExecuteWithRetry(
                () => _azureIoTHub.InvokeMethod(method));

            if (response.Outcome == OutcomeType.Successful && response.Result.Status == 200)
            {
                var stateResponse = JsonConvert.DeserializeObject<StateResponse>(response.Result.ResponsePayload);
                return stateResponse.State;
            }

            // Error
            ShowError();
            AppCenterHelper.Error(method, response.FinalException);

            return currentState;
        }

        private void ShowError()
        {
            _dialogs.Toast(new ToastConfig("Error invoking method")
            {
                BackgroundColor = System.Drawing.Color.Red,
                Position = ToastPosition.Top,
                Duration = TimeSpan.FromSeconds(3)
            });
        }
    }
}

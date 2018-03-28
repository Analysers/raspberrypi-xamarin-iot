using System;
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
    public class OtherPageModel : FreshBasePageModel
    {
        public IDeviceTwin DeviceTwin { get; set; }
        private readonly IAzureIoTHub _azureIoTHub;
        private readonly IUserDialogs _dialogs;

        public OtherPageModel(IDeviceTwin deviceTwin, IAzureIoTHub azureIoTHub, IUserDialogs dialogs)
        {
            DeviceTwin = deviceTwin;
            _azureIoTHub = azureIoTHub;
            _dialogs = dialogs;
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            await _azureIoTHub.GetDeviceTwin();

            AppCenterHelper.Track("OtherPageModel - ViewIsAppearing");

            base.ViewIsAppearing(sender, e);
        }

        public bool WakePcStatus { get; set; }
        public Command OnWakePcTapped => new Command(async () =>
        {
            AppCenterHelper.Track("OtherPageModel - OnWakePcTapped");
            WakePcStatus = true;

            var response = await ResilientCall.ExecuteWithRetry(
                () => _azureIoTHub.InvokeMethod("WakePc"));

            if (response.Outcome != OutcomeType.Successful || response.Result.Status != 200)
            {
                ShowError();
                AppCenterHelper.Error(nameof(OnWakePcTapped), response.FinalException);
            }

            WakePcStatus = false;
        });
        
        public Command OnAlarmStrobeTapped => new Command(async () =>
        {
            DeviceTwin.AlarmStrobe = await InvokeMethodWithResponseState("ToggleOutdoorAlarmStrobe", DeviceTwin.AlarmStrobe);
        });

        public Command OnAlarmSirenTapped => new Command(async () =>
        {
            DeviceTwin.AlarmSiren = await InvokeMethodWithResponseState("ToggleOutdoorAlarmSiren", DeviceTwin.AlarmSiren);
        });

        private async Task<bool> InvokeMethodWithResponseState(string method, bool currentState)
        {
            AppCenterHelper.Track($"OtherPageModel - {method}");

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

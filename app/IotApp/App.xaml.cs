using FreshMvvm;
using IotApp.Azure;
using IotApp.PageModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace IotApp
{
	public partial class App : Application
    {
        private readonly IAzureIoTHub _azureIoTHub;

        public App (IAzureIoTHub azureIoTHub)
		{
            InitializeComponent();

            _azureIoTHub = azureIoTHub;

            var seucrityPage = FreshPageModelResolver.ResolvePageModel<SecurityPageModel>();
            var lightsPage = FreshPageModelResolver.ResolvePageModel<LightsPageModel>();
            var otherPage = FreshPageModelResolver.ResolvePageModel<OtherPageModel>();

            var tabbedPage = new TabbedPage();
            tabbedPage.Children.Add(seucrityPage);
            tabbedPage.Children.Add(lightsPage);
            tabbedPage.Children.Add(otherPage);
            if (Config.DebugMode)
            {
                var deviceTwinPage = FreshPageModelResolver.ResolvePageModel<DeviceTwinPageModel>();
                tabbedPage.Children.Add(deviceTwinPage);
            }
            MainPage = tabbedPage;
		}
	}
}

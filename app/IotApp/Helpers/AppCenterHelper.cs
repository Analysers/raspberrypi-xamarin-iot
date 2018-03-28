using System;
using System.Collections.Generic;
using Microsoft.AppCenter;
using AppCenter = Microsoft.AppCenter.AppCenter;
using Analytics = Microsoft.AppCenter.Analytics.Analytics;
using Crashes = Microsoft.AppCenter.Crashes.Crashes;

namespace IotApp.Helpers
{
    public static class AppCenterHelper
    {
        public static void Initialize()
        {
            var properties = new CustomProperties();
            properties.Set("Person", GetPerson());
            AppCenter.SetCustomProperties(properties);

            AppCenter.Start("app-center-secret;", typeof(Analytics), typeof(Crashes));
        }

        public static void Track(string eventName)
        {
            Analytics.TrackEvent(eventName, new Dictionary<string,string>{{ "Person", GetPerson() } });
        }

        public static void Error(string message, Exception exception = null)
        {
            var properties = new Dictionary<string, string> {
                { "Message", message },
                { "Person", GetPerson() }
            };
            Crashes.TrackError(exception ?? new Exception("no exception"), properties);
        }

        private static string GetPerson()
        {
            var person = Config.Person;
#if IOS_SIMULATOR
            person = "Simulator"
#endif
            return person;
        }
    }
}

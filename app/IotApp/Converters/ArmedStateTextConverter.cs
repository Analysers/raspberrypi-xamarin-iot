using System;
using System.Globalization;
using IotApp.Models;
using Xamarin.Forms;

namespace IotApp.Converters
{
    public class ArmedStateTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "Disarmed";

            var state = (ArmedState)value;

            if (state.ArmedAwayDay)
                return "Armed Away Day";
            if (state.ArmedAwayNight)
                return "Armed Away Night";
            if (state.ArmedSleeping)
                return "Armed Stay Sleeping";
            if (state.Disarmed)
                return "Disarmed";

            return "Disarmed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using IotApp.Models;
using Xamarin.Forms;

namespace IotApp.Converters
{
    public class ArmedStateIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "003-shield-4.svg";

            var state = (ArmedState)value;

            if (state.ArmedAwayDay)
                return "005-shield-2.svg";
            if (state.ArmedAwayNight)
                return "005-shield-2.svg";
            if (state.ArmedSleeping)
                return "004-shield-3.svg";
            if (state.Disarmed)
                return "003-shield-4.svg";

            return "003-shield-4.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

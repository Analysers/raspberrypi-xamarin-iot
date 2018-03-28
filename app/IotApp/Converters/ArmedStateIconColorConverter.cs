using System;
using System.Globalization;
using IotApp.Models;
using Xamarin.Forms;

namespace IotApp.Converters
{
    public class ArmedStateIconColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "#009044";

            var state = (ArmedState)value;

            if (state.ArmedAwayDay)
                return "#c00101";
            if (state.ArmedAwayNight)
                return "#c00101";
            if (state.ArmedSleeping)
                return "#f26522";
            if (state.Disarmed)
                return "#009044";

            return "#009044";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

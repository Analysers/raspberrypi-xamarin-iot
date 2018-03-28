using System;
using System.Globalization;
using Xamarin.Forms;

namespace IotApp.Converters
{
    public class BoolToSwitchIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            var status = (bool)value;

            return status ? "002-switch-on.svg" : "001-switch-off.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

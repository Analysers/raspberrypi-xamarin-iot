using System;
using System.Globalization;
using Xamarin.Forms;

namespace IotApp.Converters
{
    public class BoolToStatus2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            var status = (bool)value;

            return status ? "#009044" : "#e1e1e1";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using Xamarin.Forms;

namespace IotApp.Converters
{
    public class BoolToStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            var status = (bool)value;

            return status ? "#009044" : "#c00101";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

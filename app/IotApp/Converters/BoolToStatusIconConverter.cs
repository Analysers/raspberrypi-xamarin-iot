using System;
using System.Globalization;
using Xamarin.Forms;

namespace IotApp.Converters
{
    public class BoolToStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            var status = (bool)value;

            return status ? "002-tick.svg" : "001-cross.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

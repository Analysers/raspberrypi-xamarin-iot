using System;
using System.Globalization;
using IotApp.Models;
using Xamarin.Forms;

namespace IotApp.Converters
{
    public class ArmedStateDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "Someone is home";

            var state = (ArmedState)value;

            var frontDoorArmed = $"{Environment.NewLine}Front Door notifications";
            if (state.FrontDoorArmed)
                frontDoorArmed = $"{Environment.NewLine}Front Door warn";
            
            var garageDoorArmed = "";
            if (state.GarageDoorArmed)
                garageDoorArmed = $"{Environment.NewLine}Garage Door Armed";

            if (state.ArmedAwayDay)
                return $"Indoor/Outdoor armed{frontDoorArmed}{garageDoorArmed}";
            if (state.ArmedAwayNight)
                return $"Indoor/Outdoor armed{frontDoorArmed}{garageDoorArmed}";
            if (state.ArmedSleeping)
                return $"Indoor/Outdoor armed{frontDoorArmed}{garageDoorArmed}";
            if (state.Disarmed)
                return $"Someone is home{frontDoorArmed}{garageDoorArmed}";

            return "Someone is home";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

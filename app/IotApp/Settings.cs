using System.ComponentModel;
using System.Runtime.CompilerServices;
using IotApp.Models;
using Newtonsoft.Json;
using Plugin.Settings;

namespace IotApp
{
    public class Settings : ISettings, INotifyPropertyChanged
    {
        private static Plugin.Settings.Abstractions.ISettings AppSettings => CrossSettings.Current;
        
        public OccupantState OccupantState
        {
            get => JsonConvert.DeserializeObject<OccupantState>(AppSettings.GetValueOrDefault(nameof(OccupantState), JsonConvert.SerializeObject(new OccupantState())));
            set
            {
                if (value.GetHashCode() == JsonConvert.DeserializeObject<OccupantState>(
                    AppSettings.GetValueOrDefault(nameof(OccupantState),
                        JsonConvert.SerializeObject(new OccupantState()))).GetHashCode()) return;
                
                AppSettings.AddOrUpdateValue(nameof(OccupantState), JsonConvert.SerializeObject(value));
                OnPropertyChanged(nameof(OccupantState));
            }
        }

        public ArmedState ArmedState
        {
            get => JsonConvert.DeserializeObject<ArmedState>(AppSettings.GetValueOrDefault(nameof(ArmedState), JsonConvert.SerializeObject(new ArmedState())));
            set
            {
                if (value.GetHashCode() == JsonConvert.DeserializeObject<ArmedState>(
                    AppSettings.GetValueOrDefault(nameof(ArmedState),
                        JsonConvert.SerializeObject(new ArmedState()))).GetHashCode()) return;

                AppSettings.AddOrUpdateValue(nameof(ArmedState), JsonConvert.SerializeObject(value));
                OnPropertyChanged(nameof(ArmedState));
            }
        }

        public bool AtHome
        {
            get => AppSettings.GetValueOrDefault(nameof(AtHome), true);
            set
            {
                if (value == AppSettings.GetValueOrDefault(nameof(AtHome), true)) return;

                AppSettings.AddOrUpdateValue(nameof(AtHome), value);
                OnPropertyChanged(nameof(AtHome));
            }
        }

        public bool AtBedroom
        {
            get => AppSettings.GetValueOrDefault(nameof(AtBedroom), false);
            set
            {
                if (value == AppSettings.GetValueOrDefault(nameof(AtBedroom), false)) return;

                AppSettings.AddOrUpdateValue(nameof(AtBedroom), value);
                OnPropertyChanged(nameof(AtBedroom));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

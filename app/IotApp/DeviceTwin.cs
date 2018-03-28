using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Plugin.Settings;
using IotApp.Models;

namespace IotApp
{
    public class DeviceTwin : IDeviceTwin, INotifyPropertyChanged
    {
        private static Plugin.Settings.Abstractions.ISettings AppSettings => CrossSettings.Current;

        public bool ApplianceConnected
        {
            get => AppSettings.GetValueOrDefault(nameof(ApplianceConnected), false);
            set
            {
                if (value == AppSettings.GetValueOrDefault(nameof(ApplianceConnected), false)) return;

                AppSettings.AddOrUpdateValue(nameof(ApplianceConnected), value);
                OnPropertyChanged(nameof(ApplianceConnected));
            }
        }

        public bool LightsGarden
        {
            get => AppSettings.GetValueOrDefault(nameof(LightsGarden), false);
            set
            {
                if (value == AppSettings.GetValueOrDefault(nameof(LightsGarden), false)) return;

                AppSettings.AddOrUpdateValue(nameof(LightsGarden), value);
                OnPropertyChanged(nameof(LightsGarden));
            }
        }

        public bool AlarmStrobe
        {
            get => AppSettings.GetValueOrDefault(nameof(AlarmStrobe), false);
            set
            {
                if (value == AppSettings.GetValueOrDefault(nameof(AlarmStrobe), false)) return;

                AppSettings.AddOrUpdateValue(nameof(AlarmStrobe), value);
                OnPropertyChanged(nameof(AlarmStrobe));
            }
        }

        public bool AlarmSiren
        {
            get => AppSettings.GetValueOrDefault(nameof(AlarmSiren), false);
            set
            {
                if (value == AppSettings.GetValueOrDefault(nameof(AlarmSiren), false)) return;

                AppSettings.AddOrUpdateValue(nameof(AlarmSiren), value);
                OnPropertyChanged(nameof(AlarmSiren));
            }
        }

        public OccupantState Occupant
        {
            get => JsonConvert.DeserializeObject<OccupantState>(AppSettings.GetValueOrDefault(nameof(Occupant), JsonConvert.SerializeObject(new OccupantState())));
            set
            {
                if (value.Equals(JsonConvert.DeserializeObject<OccupantState>(
                    AppSettings.GetValueOrDefault(nameof(Occupant),
                        JsonConvert.SerializeObject(new OccupantState()))))) return;

                AppSettings.AddOrUpdateValue(nameof(Occupant), JsonConvert.SerializeObject(value));
                OnPropertyChanged(nameof(Occupant));
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

        public bool GarageDoorOpen
        {
            get => AppSettings.GetValueOrDefault(nameof(GarageDoorOpen), false);
            set
            {
                if (value == AppSettings.GetValueOrDefault(nameof(GarageDoorOpen), false)) return;

                AppSettings.AddOrUpdateValue(nameof(GarageDoorOpen), value);
                OnPropertyChanged(nameof(GarageDoorOpen));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

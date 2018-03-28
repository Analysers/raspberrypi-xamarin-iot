using System.Threading.Tasks;
using Easy.Common.Extensions;
using Easy.Common.Interfaces;
using Appliance.Commands;
using Appliance.Domain;
using Appliance.Events;
using Appliance.Helpers;
using Appliance.Notifications;
using MediatR;
using Serilog;
using TimedEvent = Appliance.Enums.TimedEvent;

namespace Appliance.Components
{
    /// <summary>
    /// This is our physical sensor board
    /// Action on <see cref="Sensor.TrippedChanged"/> events for sensors
    /// Action on <see cref="MomentaryButton.Pressed"/> events for momentary push buttons
    /// </summary>
    public class SensorBoard : ISensorBoard
    {
        private readonly IMediator _mediator;
        private readonly ITimerEvents _timerEvents;
        private readonly IClock _clock;
        private readonly IAlarmState _alarmState;

        private readonly Sensor _motion;
        private readonly Sensor _tamper;

        private readonly Sensor _garageDoorSensor;
        private readonly MomentaryButton _garageDoorButton;

        public SensorBoard(IMediator mediator, ITimerEvents timerEvents, IClock clock, IAlarmState alarmState)
        {
            _mediator = mediator;
            _timerEvents = timerEvents;
            _clock = clock;
            _alarmState = alarmState;

            _motion = new Sensor(Enums.Sensor.OutdoorMotion);
            _tamper = new Sensor(Enums.Sensor.OutdoorTamper);

            _garageDoorSensor = new Sensor(Enums.Sensor.GarageDoor);
            _garageDoorButton = new MomentaryButton(Enums.Button.GarageDoor);

            Log.Information("Sensors started");
        }

        public async Task Initialize()
        {
            LogSensorReport();

            await _mediator.Send(new UpdateReportedPropertyCommand(nameof(Config.RelayState), Config.RelayState));

            _motion.TrippedChanged += async (sender, args) => { await TrippedValueChanged(sender); };
            _tamper.TrippedChanged += async (sender, args) => { await TrippedValueChanged(sender); };

            _garageDoorSensor.TrippedChanged += async (sender, args) => { await GarageDoorStateChanged(sender); };
            _garageDoorButton.Pressed += async (sender, args) => { await OnButtonPressed(sender); };
        }

        public void LogSensorReport()
        {
            Log.Information($"[SENSOR INIT] {_motion.Type} : Tripped? {_motion.Tripped}");
            Log.Information($"[SENSOR INIT] {_tamper.Type} : Tripped? {_tamper.Tripped}");
            Log.Information($"[SENSOR INIT] {_garageDoorSensor.Type} : Tripped? {_garageDoorSensor.Tripped}");
        }

        private async Task OnButtonPressed(object sender)
        {
            if (sender is MomentaryButton)
            {
                _alarmState.Reset(Enums.Sensor.GarageDoor);
                await _mediator.Publish(new PressGarageDoorRemoteButtonCommand());
            }
        }

        private async Task GarageDoorStateChanged(object sender)
        {
            if (sender is Sensor sensor)
            {
                if (_alarmState.SensorArmed(sensor.Type) && GarageDoorOpen())
                {
                    await _alarmState.Tripped(sensor.Type);
                }
                else if (GarageDoorOpen())
                {
                    _timerEvents.UpdateEvent(TimedEvent.GarageDoorOperatedEnd, new Domain.TimedEvent(_clock.Now.AddTimeSpan(5.Minutes()), true));
                }
                
                await _mediator.Send(new UpdateReportedPropertyCommand(nameof(GarageDoorOpen), GarageDoorOpen()));
            }
        }
        
        private async Task TrippedValueChanged(object sender)
        {
            if (sender is Sensor sensor)
            {
                if (sensor.Tripped)
                {
                    if (_alarmState.SensorArmed(sensor.Type))
                    {
                        await _alarmState.Tripped(sensor.Type);
                    }
                }
                else if (sensor.Type.IsSensorTamper())
                {
                    _alarmState.Reset(sensor.Type);
                }
            }
        }
        
        public bool GarageDoorOpen()
        {
            return _garageDoorSensor.Tripped;
        }
    }
}

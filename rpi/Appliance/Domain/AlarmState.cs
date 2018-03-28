using System;
using System.Threading.Tasks;
using Easy.Common.Interfaces;
using Appliance.Components;
using Appliance.Events;
using Appliance.Helpers;
using Appliance.Notifications;
using MediatR;
using Serilog;
using Sensor = Appliance.Enums.Sensor;

namespace Appliance.Domain
{
    public class AlarmState : IAlarmState
    {
        private const string FrontDoorCameraId = "000000000000000000000001";
        private const string OutdoorCameraId = "000000000000000000000002";

        private readonly IRelayBoard _relayBoard;
        private readonly ITimerEvents _timerEvents;
        private readonly IMediator _mediator;
        private readonly IClock _clock;
        private ConcurrentHashSet<Sensor> TrippedSensors { get; } = new ConcurrentHashSet<Sensor>();

        public AlarmState(IRelayBoard relayBoard, ITimerEvents timerEvents, IMediator mediator, IClock clock)
        {
            _relayBoard = relayBoard;
            _timerEvents = timerEvents;
            _mediator = mediator;
            _clock = clock;
        }

        public void Reset(Sensor sensor, bool enableAlarmActivatedLed = true)
        {
            if (sensor.IsSensorTamper() && TrippedSensors.Contains(sensor))
            {
                if (TrippedSensors.Count == 1)
                {
                    Log.Information($"[ALARMSTATE] RESET Sensor: {sensor}");
                    _relayBoard
                        .Relay(Enums.Relay.AlarmStrobe)
                        .Relay(Enums.Relay.AlarmSiren)
                        .Off();
                }

                TrippedSensors.Remove(sensor);
            }

            if (sensor.IsFrontDoor() && TrippedSensors.Contains(sensor))
            {
                if (TrippedSensors.Count == 1)
                {
                    Log.Information($"[ALARMSTATE] RESET Sensor: {sensor}");
                    _relayBoard.Off(Enums.Relay.AlarmStrobe);
                }

                TrippedSensors.Remove(sensor);
            }

            if (sensor.IsGarageDoor() && TrippedSensors.Contains(sensor))
            {
                if (TrippedSensors.Count == 1)
                {
                    Log.Information($"[ALARMSTATE] RESET Sensor: {sensor}");
                    _relayBoard
                        .Relay(Enums.Relay.AlarmStrobe)
                        .Off();
                }

                TrippedSensors.Remove(sensor);
            }

            if (sensor.IsSensorOutdoor() && TrippedSensors.Contains(sensor))
            {
                Log.Information($"[ALARMSTATE] RESET Sensor: {sensor}");
                _relayBoard
                    .Relay(Enums.Relay.AlarmStrobe)
                    .Relay(Enums.Relay.AlarmSiren)
                    .Off();

                TrippedSensors.Remove(sensor);
            }
        }

        public async Task Tripped(Sensor sensor)
        {
            var sensorDetails = SensorDetails(sensor);

            if (sensor.IsSensorTamper())
            {
                Log.Information($"[ALARMSTATE] TRIPPED Sensor: {sensor}");
                TrippedSensors.Add(sensor);
                
                _relayBoard
                    .Relay(Enums.Relay.AlarmStrobe)
                    .Relay(Enums.Relay.AlarmSiren)
                    .On();

                TurnLightsOnAtNight();

                _timerEvents.UpdateEvent(Enums.TimedEvent.StrobeOff, new TimedEvent(_clock.Now.AddTimeSpan(Config.StrobeAlarm), true));
                _timerEvents.UpdateEvent(Enums.TimedEvent.SirenOff, new TimedEvent(_clock.Now.AddTimeSpan(Config.SirenAlarm), true));

                await _mediator.Publish(new SendRichPushNotificationCommand
                {
                    Title = sensorDetails.Title,
                    Body = "Sensor has been tampered with...",
                    CameraId = sensorDetails.CameraId
                });
            }

            if (sensor.IsSensorOutdoor())
            {
                Log.Information($"[ALARMSTATE] TRIPPED Sensor: {sensor}");
                TrippedSensors.Add(sensor);

                _relayBoard
                    .Relay(Enums.Relay.AlarmStrobe)
                    .Relay(Enums.Relay.AlarmSiren)
                    .On();

                TurnLightsOnAtNight();

                _timerEvents.UpdateEvent(Enums.TimedEvent.StrobeOff, new TimedEvent(_clock.Now.AddTimeSpan(Config.StrobeAlarm), true));
                _timerEvents.UpdateEvent(Enums.TimedEvent.SirenOff, new TimedEvent(_clock.Now.AddTimeSpan(Config.SirenAlarm), true));

                await _mediator.Publish(new SendRichPushNotificationCommand
                {
                    Title = sensorDetails.Title,
                    Body = "Outdoor ALARM",
                    CameraId = sensorDetails.CameraId
                });
            }
            
            if (sensor.IsFrontDoor())
            {
                var strobeOn = "OFF";
                if (SensorArmed(Sensor.FrontDoorMotion))
                {
                    Log.Information($"[ALARMSTATE] TRIPPED Sensor: {sensor}");
                    TrippedSensors.Add(sensor);

                    _relayBoard.On(Enums.Relay.AlarmStrobe);

                    _timerEvents.UpdateEvent(Enums.TimedEvent.StrobeOff, new TimedEvent(_clock.Now.AddTimeSpan(Config.StrobeAlarm), true));

                    strobeOn = "ON";
                }
                
                await _mediator.Publish(new SendRichPushNotificationCommand
                {
                    Title = sensorDetails.Title,
                    Body = $"Alarm strobe is {strobeOn}",
                    CameraId = sensorDetails.CameraId
                });
            }

            if (sensor.IsGarageDoor())
            {
                Log.Information($"[ALARMSTATE] TRIPPED Sensor: {sensor}");
                TrippedSensors.Add(sensor);

                _relayBoard
                    .Relay(Enums.Relay.AlarmStrobe)
                    .Relay(Enums.Relay.AlarmSiren)
                    .On();

                _timerEvents.UpdateEvent(Enums.TimedEvent.StrobeOff, new TimedEvent(_clock.Now.AddTimeSpan(Config.StrobeAlarm), true));
                _timerEvents.UpdateEvent(Enums.TimedEvent.SirenOff, new TimedEvent(_clock.Now.AddTimeSpan(Config.SirenAlarm), true));

                await _mediator.Publish(new SendRichPushNotificationCommand
                {
                    Title = sensorDetails.Title,
                    Body = "Alarm ACTIVATED",
                    CameraId = sensorDetails.CameraId
                });
            }

            void TurnLightsOnAtNight()
            {
                if (Config.ArmedState.ArmedAwayNight || Config.ArmedState.ArmedSleeping)
                {
                    _relayBoard
                        .Relay(Enums.Relay.LightsGarden)
                        .On();
                }
            }
        }
        

        public bool SensorArmed(Sensor sensor)
        {
            switch (sensor)
            {
                case Sensor.OutdoorTamper:
                    return true;

                case Sensor.OutdoorMotion:
                    return Config.ArmedState.ArmedAwayDay ||
                           Config.ArmedState.ArmedAwayNight ||
                           Config.ArmedState.ArmedSleeping;

                case Sensor.GarageDoor:
                    return Config.ArmedState.GarageDoorArmed && !Config.GarageDoorOperated;

                case Sensor.FrontDoorMotion:
                    return Config.ArmedState.FrontDoorArmed;
                    
                default:
                    return false;
            }
        }

        public void OccupantStateChanged(OccupantState currentOccupantState, OccupantState newOccupantState)
        {
            if (!currentOccupantState.AtHome && newOccupantState.AtHome)
            {
                Reset(Sensor.GarageDoor, false);
            }
        }
        
        public void Disarm()
        {
            _relayBoard
                .Relay(Enums.Relay.AlarmStrobe)
                .Relay(Enums.Relay.AlarmSiren)
                .Off();
        }

        private static (string Title, string CameraId) SensorDetails(Sensor sensor)
        {
            switch (sensor)
            {
                case Sensor.OutdoorMotion:
                    return ("Outdoor - Motion", OutdoorCameraId);
                case Sensor.OutdoorTamper:
                    return ("Outdoor - Tamper", OutdoorCameraId);
                case Sensor.GarageDoor:
                    return ("Garage Door - Opened", FrontDoorCameraId);
                case Sensor.FrontDoorMotion:
                    return ("Front Door - Motion", FrontDoorCameraId);
                default:
                    return ("Unknown - Unknown", "");
            }
        }
    }
}

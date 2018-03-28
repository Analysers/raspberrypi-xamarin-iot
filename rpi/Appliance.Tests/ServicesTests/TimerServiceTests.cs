using System;
using System.Threading;
using System.Threading.Tasks;
using Easy.Common;
using Easy.Common.Extensions;
using Easy.Common.Interfaces;
using Appliance.Commands;
using Appliance.Components;
using Appliance.Controllers;
using Appliance.Events;
using Appliance.Helpers;
using Appliance.Services;
using MediatR;
using Moq;
using Xunit;

namespace Appliance.Tests.ServicesTests
{
    public class TimerServiceTests
    {
        private ITimerService _sut;
        private readonly ITimerClock _timerClock;
        private readonly Mock<IClock> _clock;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<ILedBoard> _ledBoard;
        private readonly Mock<ITimerEvents> _timerEvents;
        private readonly Mock<ILightsController> _lightsController;
        private readonly Mock<IRelayBoard> _relayBoard;

        public TimerServiceTests()
        {
            _timerClock = new TimerClock(1.Seconds());
            _clock = new Mock<IClock>();
            _mediator = new Mock<IMediator>();
            _ledBoard = new Mock<ILedBoard>();
            _timerEvents = new Mock<ITimerEvents>();
            _lightsController = new Mock<ILightsController>();
            _relayBoard = new Mock<IRelayBoard>();
        }

        [Theory]
        [InlineData(0, 0, 0, true)]
        [InlineData(1, 1, 23, false)]
        public async Task OnTheHour_ASilentPushNotificationShouldBeSent(int hour, int minutes, int seconds, bool onHour)
        {
            _sut = new TimerService(_timerClock, _clock.Object, _mediator.Object, _ledBoard.Object,
                _timerEvents.Object, _lightsController.Object, _relayBoard.Object);

            _clock.Setup(x => x.Now).Returns(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                hour, minutes, seconds));

            _sut.Start();

            await Task.Delay(2.Seconds());

            if (onHour)
                _mediator.Verify(x => x.Send(It.IsAny<PushNotificationCommand>(), CancellationToken.None), Times.AtLeastOnce);
            else
                _mediator.Verify(x => x.Send(It.IsAny<PushNotificationCommand>(), CancellationToken.None), Times.Never);
        }

        [Theory]
        [InlineData(0, 0, 25, true)]
        [InlineData(1, 1, 2, false)]
        public async Task Every5Seconds_TheRingDoorbellShouldBePolled(int hour, int minutes, int seconds, bool on5Seconds)
        {
            _sut = new TimerService(_timerClock, _clock.Object, _mediator.Object, _ledBoard.Object,
                _timerEvents.Object, _lightsController.Object, _relayBoard.Object);

            _clock.Setup(x => x.Now).Returns(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                hour, minutes, seconds));

            _sut.Start();

            await Task.Delay(2.Seconds());

            if (on5Seconds)
                _mediator.Verify(x => x.Publish(It.IsAny<PollDoorbellMotionCommand>(), CancellationToken.None), Times.AtLeastOnce);
            else
                _mediator.Verify(x => x.Publish(It.IsAny<PollDoorbellMotionCommand>(), CancellationToken.None), Times.Never);
        }

        [Theory]
        [InlineData(0, 0, 0, true)]
        [InlineData(1, 1, 23, false)]
        [InlineData(2, 0, 0, true)]
        [InlineData(3, 5, 3, false)]
        [InlineData(4, 0, 0, true)]
        [InlineData(15, 0, 0, true)]
        [InlineData(16, 56, 23, false)]
        [InlineData(23, 0, 0, true)]
        [InlineData(9, 0, 56, false)]
        [InlineData(11, 0, 0, true)]
        public void TestOnHour(int hour, int minutes, int seconds, bool onHour)
        {
            Assert.Equal(onHour, DateTimeHelpers.TimeFallsOnTheHourMark(new TimeSpan(hour, minutes, seconds)));
        }

        [Theory]
        [InlineData(0, 0, 5, true)]
        [InlineData(1, 1, 23, false)]
        [InlineData(2, 0, 0, true)]
        [InlineData(3, 5, 3, false)]
        [InlineData(4, 0, 25, true)]
        [InlineData(15, 0, 45, true)]
        [InlineData(16, 56, 23, false)]
        [InlineData(23, 0, 55, true)]
        [InlineData(9, 0, 56, false)]
        [InlineData(11, 0, 0, true)]
        public void TestOn5Seocnds(int hour, int minutes, int seconds, bool onHour)
        {
            Assert.Equal(onHour, DateTimeHelpers.TimeFallsOnThe5SecondMark(new TimeSpan(hour, minutes, seconds)));
        }
    }
}

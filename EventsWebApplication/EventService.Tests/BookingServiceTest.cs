using EventsWebApplication.Enums;
using EventsWebApplication.Models;
using WebApp = EventsWebApplication;
using EventsWebApplication.Exceptions;
using EventsWebApplication.Services.Interfaces;

namespace EventService.Tests
{
    public class BookingServiceTest
    {
        private IEventService _eventService;
        private IBookingService _bookingService;
        private IBookingQueue _bookingQueue;
        private WebApp.Services.BookingProcessService _bookingProcessService;

        public BookingServiceTest()
        {
            _eventService = new WebApp.Services.EventService();
            _bookingQueue = new WebApp.Services.BookingQueue();
            _bookingService = new WebApp.Services.BookingService(_eventService, _bookingQueue);
            _bookingProcessService = new WebApp.Services.BookingProcessService(_bookingQueue);
        }

        [Fact]
        [Trait("Category", "Success")]
        public async Task Create_Booking_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };

            //Act
            _eventService.Add(newEvent);
            var newBooking = await _bookingService.CreateBookingAsync(eventId);
            var booking = await _bookingService.GetBookingByIdAsync(newBooking.Id);

            //Assert
            Assert.Equal(BookingStatusEnum.Pending, booking?.Status);
        }

        [Fact]
        [Trait("Category", "Success")]
        public async Task Create_SeveralBookings_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };

            //Act
            _eventService.Add(newEvent);
            var newBooking1 = await _bookingService.CreateBookingAsync(eventId);
            var newBooking2 = await _bookingService.CreateBookingAsync(eventId);

            //Assert
            Assert.True(newBooking1.Id != newBooking2.Id);
        }

        [Fact]
        [Trait("Category", "Success")]
        public async Task Get_BookingById_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };

            //Act
            _eventService.Add(newEvent);
            var newBooking = await _bookingService.CreateBookingAsync(eventId);
            var booking = await _bookingService.GetBookingByIdAsync(newBooking.Id);

            //Assert
            Assert.NotNull(booking);
            Assert.Equal(BookingStatusEnum.Pending, booking?.Status);
        }

        [Fact]
        [Trait("Category", "Success")]
        public async Task Change_BookingStatus_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };

            //Act
            _eventService.Add(newEvent);
            var booking = await _bookingService.CreateBookingAsync(eventId);
            var id = booking.Id;
            var status1 = booking.Status;

            await _bookingProcessService.StartAsync(new CancellationToken());
            await Task.Delay(10000);
            await _bookingProcessService.StopAsync(new CancellationToken());

            booking = await _bookingService.GetBookingByIdAsync(id);
            var status2 = booking?.Status;

            //Assert
            Assert.Equal(BookingStatusEnum.Pending, status1);
            Assert.Equal(BookingStatusEnum.Confirmed, status2);
        }

        [Fact]
        [Trait("Category", "Unsuccess")]
        public async Task Create_BookingForUnexistEvent_Unsuccess()
        {
            //Arrange
            var eventId = Guid.NewGuid();

            //Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _bookingService.CreateBookingAsync(eventId));
        }

        [Fact]
        [Trait("Category", "Unsuccess")]
        public async Task Create_BookingForDeletedEvent_Unsuccess()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3)
            };

            //Act & Assert
            _eventService.Add(newEvent);
            _eventService.Delete(eventId);
            await Assert.ThrowsAsync<NotFoundException>(async () => await _bookingService.CreateBookingAsync(eventId));
        }

        [Fact]
        [Trait("Category", "Unsuccess")]
        public async Task Get_Booking_Unsuccess()
        {
            //Arrange
            var bookingId = Guid.NewGuid();

            //Act
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);

            //Assert
            Assert.Null(booking);
        }
    }
}

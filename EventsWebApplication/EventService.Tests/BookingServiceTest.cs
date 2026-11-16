using EventsWebApplication.Enums;
using EventsWebApplication.Exceptions;
using EventsWebApplication.Models;
using EventsWebApplication.Services;
using EventsWebApplication.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using WebApp = EventsWebApplication;

namespace EventService.Tests
{
    public class BookingServiceTest
    {
        private ILogger _logger;
        private IEventService _eventService;
        private IBookingService _bookingService;
        private IBookingQueue _bookingQueue;
        private WebApp.Services.BookingProcessService _bookingProcessService;

        public BookingServiceTest()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug());

            _logger = loggerFactory.CreateLogger<BookingProcessService>();
            _eventService = new WebApp.Services.EventService();
            _bookingQueue = new WebApp.Services.BookingQueue();
            _bookingService = new WebApp.Services.BookingService(_eventService, _bookingQueue);
            _bookingProcessService = new WebApp.Services.BookingProcessService(_bookingQueue, _eventService, _logger);
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
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = 1,
                AvailableSeats = 1
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
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = 2
            };
            newEvent.AvailableSeats = newEvent.TotalSeats;

            //Act
            _eventService.Add(newEvent);
            var newBooking1 = await _bookingService.CreateBookingAsync(eventId);
            var newBooking2 = await _bookingService.CreateBookingAsync(eventId);

            //Assert
            Assert.True(newBooking1.Id != newBooking2.Id);
        }

        [Fact]
        [Trait("Category", "Success")]
        public async Task Create_OverBooking_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = 2
            };
            newEvent.AvailableSeats = newEvent.TotalSeats;

            //Act
            _eventService.Add(newEvent);
            var newBooking1 = await _bookingService.CreateBookingAsync(eventId);
            var newBooking2 = await _bookingService.CreateBookingAsync(eventId);

            //Assert
            await Assert.ThrowsAsync<NoAvailableSeatsException>(async () => await _bookingService.CreateBookingAsync(eventId));
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
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = 2,
                AvailableSeats = 2
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
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = 1,
                AvailableSeats = 1
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
        [Trait("Category", "Success")]
        public async Task Create_Booking_MinusAvailableSeats_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = 3
            };
            newEvent.AvailableSeats = newEvent.TotalSeats;

            //Act
            _eventService.Add(newEvent);
            var booking = await _bookingService.CreateBookingAsync(eventId);
            var evnt = _eventService.GetEvent(eventId);

            //Assert
            Assert.Equal(evnt.TotalSeats - 1, evnt.AvailableSeats);
        }

        [Fact]
        [Trait("Category", "Success")]
        public async Task Change_StatusToConfirm_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = 1,
                AvailableSeats = 1
            };

            //Act
            _eventService.Add(newEvent);
            var booking = await _bookingService.CreateBookingAsync(eventId);
            booking.Confirm();

            //Assert
            Assert.Equal(BookingStatusEnum.Confirmed, booking.Status);
            Assert.NotNull(booking.ProcessedAt);
        }

        [Fact]
        [Trait("Category", "Success")]
        public async Task Change_StatusToRejected_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var seatsCount = 1;
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = seatsCount,
                AvailableSeats = seatsCount
            };

            //Act
            _eventService.Add(newEvent);
            var booking = new Booking(_eventService);
            var bookingCreated = await _bookingService.CreateBookingAsync(eventId);

            booking.Id = bookingCreated.Id;
            booking.EventId = bookingCreated.EventId;
            booking.CreatedAt = bookingCreated.CreatedAt;
            booking.ProcessedAt = bookingCreated.ProcessedAt;
            booking.Status = bookingCreated.Status;

            booking.Reject();

            //Assert
            Assert.Equal(BookingStatusEnum.Rejected, booking.Status);
            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(seatsCount, newEvent.AvailableSeats);
        }

        [Fact]
        [Trait("Category", "Success")]
        public async Task Create_BookingAfterReject_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var seatsCount = 1;
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = seatsCount,
                AvailableSeats = seatsCount
            };

            //Act
            _eventService.Add(newEvent);
            var booking = new Booking(_eventService);
            var bookingCreated = await _bookingService.CreateBookingAsync(eventId);

            booking.Id = bookingCreated.Id;
            booking.EventId = bookingCreated.EventId;
            booking.CreatedAt = bookingCreated.CreatedAt;
            booking.ProcessedAt = bookingCreated.ProcessedAt;
            booking.Status = bookingCreated.Status;

            booking.Reject();

            var bookingAfterReject = await _bookingService.CreateBookingAsync(eventId);

            //Assert
            Assert.NotNull(bookingAfterReject);
        }

        [Fact]
        [Trait("Category", "Success")]
        public async Task Defend_FromOverbooking_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var seatsCount = 5;
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = seatsCount,
                AvailableSeats = seatsCount
            };
            var bookingCount = 0;
            var exceptionCount = 0;

            //Act
            _eventService.Add(newEvent);
            await Parallel.ForAsync(0, 20, async(i, token) =>
            {
                await Task.Delay(1000);
                try
                {
                    await _bookingService.CreateBookingAsync(eventId);
                    Interlocked.Increment(ref bookingCount);
                }
                catch (NoAvailableSeatsException ex)
                {
                    Interlocked.Increment(ref exceptionCount);
                }
            });

            //Assert
            Assert.Equal(5, bookingCount);
            Assert.Equal(15, exceptionCount);
            Assert.Equal(0, newEvent.AvailableSeats);
        }

        [Fact]
        [Trait("Category", "Success")]
        public async Task Create_BookingWithUniqueId_Success()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var seatsCount = 10;
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = seatsCount,
                AvailableSeats = seatsCount
            };
            var list = new ConcurrentBag<Guid>();

            //Act
            _eventService.Add(newEvent);
            await Parallel.ForAsync(0, 10, async (i, token) =>
            {
                try
                {
                    var booking = await _bookingService.CreateBookingAsync(eventId);
                    list.Add(booking.Id);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            });
            bool isUnique = list.All(new HashSet<Guid>().Add);

            //Assert
            Assert.True(isUnique);
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

        [Fact]
        [Trait("Category", "Unsuccess")]
        public async Task Booking_NoAvailableSeats_Unsuccess()
        {
            //Arrange
            var eventId = Guid.NewGuid();
            var newEvent = new Event
            {
                Id = eventId,
                Title = "Event1",
                Description = string.Empty,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(3),
                TotalSeats = 2
            };

            //Act
            newEvent.AvailableSeats = 0;
            _eventService.Add(newEvent);

            //Assert
            await Assert.ThrowsAsync<NoAvailableSeatsException>(async () => await _bookingService.CreateBookingAsync(eventId));
        }
    }
}

using EventsWebApplication.Models;
using EventsWebApplication.Services.Interfaces;

namespace EventsWebApplication.Services
{
    public class BookingProcessService : BackgroundService
    {
        private readonly IBookingQueue _bookingQueue;
        private readonly IEventService _eventService;
        private readonly ILogger<BookingProcessService> _logger;

        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

        public BookingProcessService(
            IBookingQueue bookingQueue,
            IEventService eventService,
            ILogger<BookingProcessService> logger)
        {
            _bookingQueue = bookingQueue;
            _eventService = eventService;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var pendingBookings = _bookingQueue.GetPending().ToList();
                    var tasks = pendingBookings.Select(booking => ProcessBookingAsync(booking, stoppingToken));
                    await Task.WhenAll(tasks);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e.Message);
                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

        private async Task ProcessBookingAsync(Booking booking, CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

            await _processingSemaphore.WaitAsync();

            var evnt = _eventService.GetEvent(booking.EventId);

            try
            {
                if (evnt == null)
                {
                    booking.Reject();

                    _logger.LogInformation("Warning");
                }
                else
                {
                    booking.Confirm();
                }

                _bookingQueue.Update(booking);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                booking.Reject();
                _bookingQueue.Update(booking);

                evnt.ReleaseSeats();
                _eventService.Update(evnt.Id, evnt);
            }
            finally
            {
                _processingSemaphore.Release();
            }
        }
    }
}

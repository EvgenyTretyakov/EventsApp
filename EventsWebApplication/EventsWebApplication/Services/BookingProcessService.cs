using EventsWebApplication.Enums;
using EventsWebApplication.Services.Interfaces;

namespace EventsWebApplication.Services
{
    public class BookingProcessService : BackgroundService
    {
        private readonly IBookingQueue _bookingQueue;

        public BookingProcessService(IBookingQueue bookingQueue)
        {
            _bookingQueue = bookingQueue;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_bookingQueue.TryDequeue(out var bookingId))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

                        _bookingQueue.UpdateStatus(bookingId);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}

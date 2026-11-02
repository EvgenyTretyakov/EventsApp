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
                    if (!_bookingQueue.AnyToProcess())
                    {
                        continue;
                    }

                    if (_bookingQueue.TryDequeue(out var booking))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                        booking.Status = BookingStatusEnum.Confirmed;
                        booking.ProcessedAt = DateTime.Now;

                        _bookingQueue.Enqueue(booking);
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

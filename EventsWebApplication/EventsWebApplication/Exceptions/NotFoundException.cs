using System;

namespace EventsWebApplication.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string? message)
            :base(message)
        {
        }
    }
}

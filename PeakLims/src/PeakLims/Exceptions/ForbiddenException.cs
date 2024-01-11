namespace PeakLims.Exceptions
{
    using System;

    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException() : base() { }

        public ForbiddenAccessException(string message) : base(message) { }
    }
}
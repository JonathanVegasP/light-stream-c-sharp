using System;

namespace LightStream.Exceptions
{
    public class LightStreamException : Exception
    {
        public LightStreamException(string message) : base(message)
        {
        }
    }
}

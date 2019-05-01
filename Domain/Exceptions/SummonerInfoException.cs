using System;

namespace Domain.Exceptions
{
    public class SummonerInfoException : Exception
    {
        public SummonerInfoException(string message) : base(message)
        {
        }

        public SummonerInfoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

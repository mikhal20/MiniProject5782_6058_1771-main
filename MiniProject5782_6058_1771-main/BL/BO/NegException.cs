using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class NegException : Exception
    {
        public NegException()
        {
        }

        public NegException(string message) : base(message)
        {
        }

        public NegException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NegException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
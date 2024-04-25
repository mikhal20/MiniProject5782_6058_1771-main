using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class IDException : Exception
    {
        public IDException()
        {
        }

        public IDException(string message) : base(message)
        {
        }

        public IDException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IDException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
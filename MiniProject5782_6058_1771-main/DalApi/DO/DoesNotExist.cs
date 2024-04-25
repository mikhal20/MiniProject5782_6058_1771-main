using System;
using System.Runtime.Serialization;

namespace DO
{
    [Serializable]
    public class DoesNotExist : Exception
    {
        public DoesNotExist()
        {
        }

        public DoesNotExist(string message) : base(message)
        {
        }

        public DoesNotExist(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DoesNotExist(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
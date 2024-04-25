using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class NoAvailableChargeSlotsException : Exception
    {
        public NoAvailableChargeSlotsException()
        {
        }

        public NoAvailableChargeSlotsException(string message) : base(message)
        {
        }

        public NoAvailableChargeSlotsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoAvailableChargeSlotsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
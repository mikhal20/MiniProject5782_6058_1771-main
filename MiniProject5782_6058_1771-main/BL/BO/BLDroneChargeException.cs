using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class BLDroneChargeException : Exception
    {
        public BLDroneChargeException()
        {
        }

        public BLDroneChargeException(string message) : base(message)
        {
        }

        public BLDroneChargeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BLDroneChargeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class BLDoesNotExist : Exception
    {
        public BLDoesNotExist()
        {
        }

        public BLDoesNotExist(string message) : base(message)
        {
        }

        public BLDoesNotExist(string message, Exception innerException) : base(message, innerException)
        {
        }


        protected BLDoesNotExist(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
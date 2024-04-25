using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
   public class BLAlreadyExist : Exception
    {
        public BLAlreadyExist()
        {
        }

        public BLAlreadyExist(string message) : base(message)
        {
        }

        public BLAlreadyExist(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BLAlreadyExist(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
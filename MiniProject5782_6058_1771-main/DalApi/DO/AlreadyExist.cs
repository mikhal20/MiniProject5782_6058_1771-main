using System;
using System.Runtime.Serialization;

namespace DO
{
    [Serializable]
   public class AlreadyExist : Exception
    {
        public AlreadyExist()
        {
        }

        public AlreadyExist(string message) : base(message)
        {
        }

        public AlreadyExist(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AlreadyExist(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
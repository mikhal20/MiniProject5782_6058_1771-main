using System;
using System.Runtime.Serialization;

namespace Dal
{
    [Serializable]
    public class XmlAlreadyExistException : Exception
    {
        public XmlAlreadyExistException()
        {
        }

        public XmlAlreadyExistException(string message) : base(message)
        {
        }

        public XmlAlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected XmlAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
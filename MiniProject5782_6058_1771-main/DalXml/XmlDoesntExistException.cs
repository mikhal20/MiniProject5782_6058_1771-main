using System;
using System.Runtime.Serialization;

namespace Dal
{
    [Serializable]
    public class XmlDoesntExistException : Exception
    {
        public XmlDoesntExistException()
        {
        }

        public XmlDoesntExistException(string message) : base(message)
        {
        }

        public XmlDoesntExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected XmlDoesntExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
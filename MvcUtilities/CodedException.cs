using System;
using System.Net;
using System.Runtime.Serialization;

namespace MvcUtilities
{
    [Serializable]
    public abstract class CodedException : Exception
    {
        public CodedException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public CodedException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public CodedException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        protected CodedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            StatusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode));
        }

        public HttpStatusCode StatusCode { get; private set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StatusCode", StatusCode);
        }
    }
}

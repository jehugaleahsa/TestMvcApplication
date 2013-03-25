using System;
using System.Net;
using System.Runtime.Serialization;

namespace Adapters
{
    [Serializable]
    public class AdapterException : Exception
    {
        public AdapterException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public AdapterException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public AdapterException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        protected AdapterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            StatusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StatusCode", StatusCode);
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}

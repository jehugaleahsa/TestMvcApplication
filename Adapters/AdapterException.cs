using System;
using System.Net;
using MvcUtilities;

namespace Adapters
{
    [Serializable]
    public class AdapterException : CodedException
    {
        public AdapterException(HttpStatusCode statusCode)
            : base(statusCode)
        {
        }

        public AdapterException(HttpStatusCode statusCode, string message)
            : base(statusCode, message)
        {
        }

        public AdapterException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(statusCode, message, innerException)
        {
        }
    }
}

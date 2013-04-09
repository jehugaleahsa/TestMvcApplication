using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;

namespace Adapters.Tests
{
    [TestClass]
    public class AdapterExceptionTester
    {
        [TestMethod]
        public void ShouldBeSerializable()
        {
            Exception innerException = new Exception("inner exception message");
            AdapterException exception = new AdapterException(HttpStatusCode.InternalServerError, "exception message", innerException);

            SerializationHelper helper = new SerializationHelper();
            AdapterException deserialized = helper.RoundTrip(exception);

            Assert.AreEqual(exception.StatusCode, deserialized.StatusCode, "The status code was not serialized.");
            Assert.AreEqual(exception.Message, deserialized.Message, "The message was not serialized.");
            Assert.AreEqual(innerException.Message, deserialized.InnerException.Message, "The inner exception was not serialized.");
        }

        [TestMethod]
        public void ShouldSetHttpStatusCode()
        {
            AdapterException exception = new AdapterException(HttpStatusCode.InternalServerError);
            Assert.AreEqual(HttpStatusCode.InternalServerError, exception.StatusCode, "The status code was not set.");
        }

        [TestMethod]
        public void ShouldSetHttpStatusCodeAndMessage()
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            const string message = "error message";
            AdapterException exception = new AdapterException(statusCode, message);
            Assert.AreEqual(statusCode, exception.StatusCode, "The status code was not set.");
            Assert.AreEqual(message, exception.Message, "The message was not set.");
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;

namespace ServiceInterfaces.Tests
{
    [TestClass]
    public class ServiceExceptionTester
    {
        [TestMethod]
        public void ShouldBeSerializeable()
        {
            Exception innerException = new Exception("Inner Error Message");
            const string message = "Error message";
            ServiceException exception = new ServiceException(message, innerException);

            SerializationHelper helper = new SerializationHelper();
            ServiceException deserialized = helper.RoundTrip(exception);

            Assert.AreEqual(message, deserialized.Message, "The message was not serialized.");
            Assert.AreEqual(innerException.Message, deserialized.InnerException.Message, "The inner exception was not serialized.");
        }

        [TestMethod]
        public void ShouldStoreMessagePassedToConstructor()
        {
            const string message = "Error Message";
            ServiceException exception = new ServiceException(message);
            Assert.AreEqual(message, exception.Message);
        }

        [TestMethod]
        public void ShouldProvideDefaultMessage()
        {
            ServiceException serviceException = new ServiceException();
            Assert.AreEqual("Exception of type 'ServiceInterfaces.ServiceException' was thrown.", serviceException.Message);
        }
    }
}

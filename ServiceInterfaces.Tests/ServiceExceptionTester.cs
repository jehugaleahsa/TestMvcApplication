using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
            ServiceException deserialized = serializeRoundTrip(exception);

            Assert.AreEqual(message, deserialized.Message, "The message was not serialized.");
            Assert.AreEqual(innerException.Message, deserialized.InnerException.Message, "The inner exception was not serialized.");
        }

        private static ServiceException serializeRoundTrip(ServiceException exception)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, exception);
                stream.Flush();
                stream.Position = 0;
                return (ServiceException)formatter.Deserialize(stream);
            }
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

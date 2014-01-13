using System;
using System.Net;
using Adapters.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adapters.Tests
{
    [TestClass]
    public class PrimitiveMapperTester
    {
        [TestMethod]
        public void ShouldThrowAdapterExceptionForInvalidGuid()
        {
            try
            {
                PrimitiveMapper mapper = new PrimitiveMapper();
                mapper.ToGuid("123");
                Assert.Fail("The invalid GUID should have caused an exception to be thrown.");
            }
            catch (AdapterException exception)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode, "The wrong status code was sent.");
            }
        }

        [TestMethod]
        public void ShouldConvertValidStringIntoGuid()
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            Guid guid = Guid.NewGuid();
            Guid actual = mapper.ToGuid(mapper.ToString(guid));
            Assert.AreEqual(guid, actual, "The GUID was not parsed correctly.");
        }

        [TestMethod]
        public void ShouldThrowAdapterExceptionForInvalidDate()
        {
            try
            {
                PrimitiveMapper mapper = new PrimitiveMapper();
                mapper.ToDateTime("hello");
                Assert.Fail("The invalid date/time should have caused an exception to be thrown.");
            }
            catch (AdapterException exception)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode, "The wrong status code was sent.");
            }
        }

        [TestMethod]
        public void ShouldCovertValidStringToDate()
        {
            PrimitiveMapper mapper = new PrimitiveMapper();
            DateTime original = DateTime.Today;  // date only
            DateTime actual = mapper.ToDateTime(mapper.ToString(original));
            Assert.AreEqual(original, actual, "The date/time was not parsed correctly.");
        }
    }
}

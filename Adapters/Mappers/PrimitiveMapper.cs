using System;
using System.Net;

namespace Adapters.Mappers
{
    public class PrimitiveMapper
    {
        public Guid ToGuid(string value)
        {
            try
            {
                return Guid.ParseExact(value, "N");
            }
            catch (Exception exception)
            {
                throw new AdapterException(HttpStatusCode.BadRequest, "Encountered an invalid ID.", exception);
            }
        }

        public DateTime ToDateTime(string value)
        {
            try
            {
                return DateTime.Parse(value);
            }
            catch (Exception exception)
            {
                throw new AdapterException(HttpStatusCode.BadRequest, "Encountered an invalid date.", exception);
            }
        }

        public string ToString(DateTime value)
        {
            return value.ToString("d");
        }

        public string ToString(Guid value)
        {
            return value.ToString("N");
        }
    }
}

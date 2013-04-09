using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TestUtilities
{
    public class SerializationHelper
    {
        public T RoundTrip<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Flush();
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}

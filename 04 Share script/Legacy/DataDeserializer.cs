using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataDeserializer
{
    public static T Deserialize<T>(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            return (T)formatter.Deserialize(stream);
        }
    }
}

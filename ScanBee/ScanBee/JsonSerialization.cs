using System;
using System.Runtime.Serialization.Json;
using System.IO;

namespace ScanBee
{
    public class JsonSerialization
    {
        public T deserializeJSON<T>(string json)
        {
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            {
                sw.Write(json);
                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));
                return (T)deserializer.ReadObject(ms);
            }
        }

        public string Serialize(Object Obj)
        {
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(Obj.GetType());
            ser.WriteObject(stream1, Obj);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return sr.ReadToEnd();

        }
    }
}

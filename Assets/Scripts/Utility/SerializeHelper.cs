using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializeHelper {

    private static BinaryFormatter bf_;
    private static BinaryFormatter binary_formatter_
    {
        get
        {
            if(bf_ == null) { bf_ = new BinaryFormatter(); }
            return bf_;
        }
    }

    public static byte[] ObjectToByteArraySerialize(object obj)
    {
        using (var memoryStream = new MemoryStream())
        {
            var bf = binary_formatter_;
            bf.Serialize(memoryStream, obj);
            memoryStream.Flush();
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }
    }

    public static T Deserialize<T>(byte[] byteData)
    {
        using (var stream = new MemoryStream(byteData))
        {
            var bf = binary_formatter_;
            stream.Seek(0, SeekOrigin.Begin);
            return (T)bf.Deserialize(stream);
        }
    }
}

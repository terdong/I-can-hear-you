using LZ4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public class CompressHelper : MonoBehaviour {
    public static byte[] Compress(byte[] buffer)
    {
        //LZ4
        using (MemoryStream memoryStream = new MemoryStream())
        {
            //memoryStream.Write(buffer, 0, buffer.Length);
            //memoryStream.Position = 0;

            using (LZ4Stream lz4Stream = new LZ4Stream(memoryStream, CompressionMode.Compress, LZ4StreamFlags.HighCompression))
            {
                lz4Stream.Write(buffer, 0, buffer.Length);
            }

            //LZ4File lz4File = new LZ4File();
            //lz4File.Save(memoryStream);
            return memoryStream.ToArray();
        }

        //byte[] compressedByte;
        //using (var memoryStream = new MemoryStream())
        //{
        //    using (var ds = new DeflateStream(memoryStream, CompressionMode.Compress))
        //    //using (var writer = new StreamWriter(ds))
        //    {
        //        ds.Write(buffer, 0, buffer.Length);
        //    }
        //    compressedByte =  memoryStream.ToArray();
        //}

        //return compressedByte;
        // byte[] compressedByte;
        //using (MemoryStream ms = new MemoryStream())
        //{
        //   using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress))
        //   {
        //       ds.Write(buffer, 0, buffer.Length);
        //   }

        //   compressedByte = ms.ToArray();
        //}
        // return compressedByte;
    }
    public static byte[] Decompress(byte[] buffer)
    {

        //memoryStream.Write(buffer, 0, buffer.Length);
        //memoryStream.Position = 0;

        using (LZ4Stream lz4Stream = new LZ4Stream(new MemoryStream(buffer), CompressionMode.Decompress))
            {
                //lz4Stream.Read(buffer, 0, buffer.Length);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                CopyStream(lz4Stream, memoryStream);
                //lz4Stream.Read(buffer, 0, buffer.Length);
                return memoryStream.ToArray();

            }
        }

            //LZ4File lz4File = new LZ4File();
            //lz4File.Save(memoryStream);


        //MemoryStream resultStream = new MemoryStream();

        //using (MemoryStream ms = new MemoryStream(buffer))
        //{
        //    using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
        //    using (StreamReader reader = new StreamReader(ds))
        //    {
        //        //CopyStream(ds, resultStream);
        //        //ds.Close();
        //        reader.ReadToEnd();
        //        return ms.ToArray();
        //    }
        //}
        //byte[] decompressedByte = resultStream.ToArray();
        //resultStream.Dispose();
        //return decompressedByte;
    }

    private static void CopyStream(Stream from, Stream to)
    {
        int bufSize = 1024, count;
        byte[] buffer = new byte[bufSize];
        count = from.Read(buffer, 0, bufSize);
        while (count > 0)
        {
            to.Write(buffer, 0, count);
            count = from.Read(buffer, 0, bufSize);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public class CompressHelper : MonoBehaviour {
    public static byte[] Compress(byte[] buffer)
    {
        byte[] compressedByte;
        using (MemoryStream ms = new MemoryStream())
        {
            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress))
            {
                ds.Write(buffer, 0, buffer.Length);
            }

            compressedByte = ms.ToArray();
        }
        return compressedByte;
    }
    public static byte[] Decompress(byte[] buffer)
    {
        MemoryStream resultStream = new MemoryStream();

        using (MemoryStream ms = new MemoryStream(buffer))
        {
            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
            {
                CopyStream(ds, resultStream);
                ds.Close();
            }
        }
        byte[] decompressedByte = resultStream.ToArray();
        resultStream.Dispose();
        return decompressedByte;
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

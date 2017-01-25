using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MarshalConverter
{
    public static byte[] ToByteArray<T>(T[] source) where T : struct
    {
        GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
        try
        {
            IntPtr pointer = handle.AddrOfPinnedObject();
            byte[] destination = new byte[source.Length * Marshal.SizeOf(typeof(T))];
            Marshal.Copy(pointer, destination, 0, destination.Length);
            return destination;
        }
        finally
        {
            if (handle.IsAllocated)
                handle.Free();
        }
    }

    public static T[] FromByteArray<T>(byte[] source) where T : struct
    {
        T[] destination = new T[source.Length / Marshal.SizeOf(typeof(T))];
        GCHandle handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
        try
        {
            IntPtr pointer = handle.AddrOfPinnedObject();
            Marshal.Copy(source, 0, pointer, source.Length);
            return destination;
        }
        finally
        {
            if (handle.IsAllocated)
                handle.Free();
        }
    }
}

[Obsolete]
public class ColorConverter {

    public static Color32 UIntToColor(uint color)
    {
        //byte a = (byte)(color >> 24);
        byte r = (byte)(color >> 16);
        byte g = (byte)(color >> 8);
        byte b = (byte)(color >> 0);

        return new Color32(r, g, b, 255);
    }

    //public static Color32[] ByteArrayToColorArray(byte[])
    //{

    //}

    public static byte[] ColorToByteArray(Color32[] colors)
    {
        if (colors == null || colors.Length == 0)
            return null;

        int lengthOfColor32 = Marshal.SizeOf(typeof(Color32));
        int length = lengthOfColor32 * colors.Length;
        byte[] bytes = new byte[length];

        GCHandle handle = default(GCHandle);
        try
        {
            handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();
            Marshal.Copy(ptr, bytes, 0, length);
        }
        finally
        {
            if (handle != default(GCHandle))
                handle.Free();
        }
        return bytes;
    }

    public static uint ColorToUInt(Color32 color)
    {
        return (uint)((color.r << 16) | (color.g << 8) | (color.b << 0));
    }

    public static uint[] ColorArrayToUIntArray(Color32 [] colors)
    {
        uint[] uint_array = new uint[colors.Length];
        int length = colors.Length;
        for (int i = 0; i < length; ++i)
        {
            uint_array[i] = ColorToUInt(colors[i]);
        }

        return uint_array;
    }
}

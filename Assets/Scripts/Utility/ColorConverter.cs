using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorConverter {

    public static Color32 UIntToColor(uint color)
    {
        //byte a = (byte)(color >> 24);
        byte r = (byte)(color >> 16);
        byte g = (byte)(color >> 8);
        byte b = (byte)(color >> 0);

        return new Color32(r, g, b, 255);
    }

    public static uint ColorToUInt(Color32 color)
    {
        return (uint)((color.r << 16) | (color.g << 8) | (color.b << 0));
    }
}

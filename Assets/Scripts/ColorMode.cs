using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Explicit)]
public struct ColorMode
{
    [FieldOffset(0)]
    public bool index;

    [FieldOffset(1)]
    public int colorIndex;

    [FieldOffset(1)]
    public Color32 customColor;

    public ColorMode(int colorIndex) : this()
    {
        index = true;
        this.colorIndex = colorIndex;
    }

    public ColorMode(Color32 customColor) : this()
    {
        index = false;
        this.customColor = customColor;
    }
}

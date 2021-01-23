using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Compression;
using System.Numerics;

public class BitReader
{
    private readonly byte[] buf;
    private ulong pos;

    public BitReader(Reader reader)
    {
        int uncompressed = reader.ReadInt();
        int compressed = reader.ReadInt();

        if (compressed == 0)
        {
            buf = reader.ReadNBytes(uncompressed);
        }
        else
        {
            reader.SkipNBytes(2); // Remove 2 byte prefix to fix stuff???
            DeflateStream decompressStream = new DeflateStream(reader.GetStream(), CompressionMode.Decompress);
            buf = new byte[uncompressed];
            decompressStream.Read(buf, 0, uncompressed);
        }
        pos = 0;
    }

    public void ByteAlign()
    {
        pos = (pos + 7) & (~((ulong)0x07));
    }

    public bool ReadBit()
    {
        bool bit = (buf[pos >> 3] & (1 << (int)(pos & 7))) != 0;
        pos++;
        return bit;
    }

    public void ReadBits(byte[] dst, int len)
    {
        for (uint bit=0; bit < len; bit++)
        {
            uint shift = bit & 0x07;
            dst[bit >> 3] = (byte)(dst[bit >> 3] & ~(1 << (int)shift) | ((ReadBit()?1:0) << (int)shift));
        }
    }

    public int ReadInt()
    {
        byte[] bytes = new byte[4];
        ReadBits(bytes, 32);
        return bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0];
    }

    public int ReadThree()
    {
        byte[] bytes = new byte[3];
        ReadBits(bytes, 24);
        return bytes[2] << 16 | bytes[1] << 8 | bytes[0];
    }

    public int ReadInt(int max)
    {
        int value = 0;
        int mask = 1;
        while ((value + mask) < max && mask != 0)
        {
            if (ReadBit())
                value |= mask;
            mask <<= 1;
        }
        return value;
    }

    public int ReadIntPacked()
    {
        int value = 0;
        for (int i = 0; i < 5; i++)
        {
            bool hasNext = ReadBit();
            int part = 0;
            for (int bitShift = 0; bitShift < 7; bitShift++)
                part |= (ReadBit() ? 1 : 0) << bitShift;
            value |= part << (7 * i);
            if (!hasNext)
                break;
        }
        return value;
    }

    private int ReadSignedIntPacked()
    {
        uint value = (uint)ReadIntPacked();
        return ((int)(value >> 1)) * ((value & 1) != 0 ? 1 : -1);
    }

    public Vector3Int ReadPositiveIntVectorPacked()
    {
        int x = ReadIntPacked();
        int y = ReadIntPacked();
        int z = ReadIntPacked();
        return new Vector3Int(x, z, y);
    }

    public Vector3Int ReadIntVectorPacked()
    {
        int x = ReadSignedIntPacked();
        int y = ReadSignedIntPacked();
        int z = ReadSignedIntPacked();
        return new Vector3Int(x, z, -y);
    }

}

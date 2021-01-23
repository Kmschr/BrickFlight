﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.IO.Compression;
using UnityEngine;

public class Reader
{
    private readonly Stream stream;

    public Reader(Stream stream)
    {
        this.stream = stream;
    }

    public Reader DecompressSection()
    {
        int uncompressed = ReadInt();
        int compressed = ReadInt();

        if (compressed == 0)
        {
            return this;
        } else
        {
            SkipNBytes(2); // Remove 2 byte prefix to fix stuff???
            byte[] compressedBuf = new byte[compressed-2];
            stream.Read(compressedBuf, 0, compressed-2);
            Stream compressedStream = new MemoryStream(compressedBuf);
            DeflateStream decompressStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
            byte[] uncompressedBuf = new byte[uncompressed];
            decompressStream.Read(uncompressedBuf, 0, uncompressed);
            return new Reader(new MemoryStream(uncompressedBuf));
        } 
    }

    public Stream GetStream()
    {
        return stream;
    }

    public byte[] ReadNBytes(int n)
    {
        byte[] buf = new byte[n];
        stream.Read(buf, 0, n);
        return buf;
    }

    public void SkipNBytes(int n)
    {
        stream.Seek(n, SeekOrigin.Current);
    }

    public int ReadShort()
    {
        byte[] buf = ReadNBytes(2);
        return buf[1] << 8 | buf[0]; 
    }

    public int ReadInt()
    {
        byte[] buf = ReadNBytes(4);
        return buf[3] << 24 | buf[2] << 16 | buf[1] << 8 | buf[0];
    }

    public long ReadLong()
    {
        byte[] buf = ReadNBytes(8);
        return buf[7] << 56 | buf[6] << 48 | buf[5] << 40 | buf[4] << 32 | buf[3] << 24 | buf[2] << 16 | buf[1] << 8 | buf[0];
    }

    public string ReadString()
    {
        int len = ReadInt();
        if (len < 0)
        {
            len = -len;
            SkipNBytes(len);
            return "";
        }
        if (len == 0)
        {
            return "";
        }
        byte[] buf = ReadNBytes(len-1);
        SkipNBytes(1);
        return System.Text.Encoding.ASCII.GetString(buf);
    }

    public Color32 ReadColor()
    {
        int b = stream.ReadByte();
        int g = stream.ReadByte();
        int r = stream.ReadByte();
        int a = stream.ReadByte();
        return new Color32((byte) (Mathf.LinearToGammaSpace(r / 255f) * 255f), 
                            (byte) (Mathf.LinearToGammaSpace(g / 255f) * 255f), 
                            (byte) (Mathf.LinearToGammaSpace(b / 255f) * 255f), 
                            (byte) (Mathf.LinearToGammaSpace(a / 255f) * 255f));
    }

    public List<T> Array<T>(Func<T> func) 
    {
        List<T> array = new List<T>();
        int len = ReadInt();
        for (int i=0; i < len; ++i)
        {
            array.Add(func());
        }
        return array;
    }

}

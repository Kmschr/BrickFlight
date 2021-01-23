using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BRS : Singleton<BRS>
{
    private static string BUILDS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Brickadia\\Saved\\Builds\\";
    private static byte[] MAGIC = { (byte)'B', (byte)'R', (byte)'S' };

    [ReadOnly]
    public string path;
    [ReadOnly]
    public int brsVersion = 0;
    [ReadOnly]
    public int gameVersion = 0;
    [ReadOnly]
    public string map;
    [ReadOnly]
    public string author;
    [ReadOnly]
    public string description;
    [ReadOnly]
    public int brickCount = 0;

    public string buildName;
    public List<string> brickAssets;
    public List<string> materials;
    public List<Color32> colors;
    public List<String> physMats;
    public Material baseMat; 
    public bool generateColliders;
    public int chunksPerFrame = 10;

    private Reader brs;
    private Texture2D image;

    public Texture2D GetPreview()
    {
        return image;
    }

    public static void ClearBricks()
    {
        GameObject bricks = GameObject.Find("Bricks");
        DestroyImmediate(bricks);
    }

    public void LoadBRS()
    {
        Debug.Log("loadBRS()");
        ClearBricks();

        // combine name and path and turn into file info object
        string filePath = BUILDS_PATH + buildName + ".brs";
        FileInfo brsInfo = new FileInfo(filePath);
        path = filePath;

        // Make sure save exists and is a brs
        if (!brsInfo.Exists || !brsInfo.Extension.Equals(".brs"))
        {
            Debug.LogWarning("Could not find save specified");
            Debug.LogWarning(filePath);
            return;
        }

        // Turn into a filestream for reading
        brs = new Reader(File.OpenRead(filePath));
        ReadPreamble();
        ReadHeader1();
        ReadHeader2();
        ReadScreenshot();
        ReadBricks();
    }

    private void ReadPreamble()
    {
        byte[] magic = brs.ReadNBytes(3);
        if (!Enumerable.SequenceEqual(magic, MAGIC))
        {
            Debug.LogWarning("Invalid starting bytes for BRS");
            return;
        }

        brsVersion = brs.ReadShort();
        if (brsVersion >= 8)
            gameVersion = brs.ReadInt();
    }

    private void ReadHeader1()
    {
        //Debug.Log("Reading Header1");
        Reader h1 = brs.DecompressSection();
        map = h1.ReadString();
        author = h1.ReadString();
        description = h1.ReadString();
        h1.SkipNBytes(16); // Author UUID
        if (brsVersion >= 8)
        {
            h1.ReadString(); // Host Name
            h1.SkipNBytes(16); // Host UUID
        }
        h1.SkipNBytes(8); // Time
        brickCount = h1.ReadInt();
    }

    private void ReadHeader2()
    {
        //Debug.Log("Reading Header2");
        Reader h2 = brs.DecompressSection();
        List<string> mods = h2.Array(() => h2.ReadString());
        brickAssets = h2.Array(() => h2.ReadString());
        colors = h2.Array(() => h2.ReadColor());
        materials = h2.Array(() => h2.ReadString());
        int users = h2.ReadInt();
        for (int i = 0; i < users; ++i)
        {
            h2.SkipNBytes(16); // UUID
            h2.ReadString();
            if (brsVersion >= 8)
            {
                h2.ReadInt();
            }
        }
        if (brsVersion >= 9)
        {
            physMats = h2.Array(() => h2.ReadString());
        }
    }

    private void ReadScreenshot()
    {
        if (brsVersion >= 8)
        {
            if (brs.ReadNBytes(1)[0] != 0)
            {
                int len = brs.ReadInt();
                byte[] fileData = brs.ReadNBytes(len);
                image = new Texture2D(2, 2);
                image.LoadImage(fileData);
            }
        }
    }

    private void ReadBricks()
    {
        BrickWorld world = new BrickWorld();
        BitReader bw = new BitReader(brs);

        int numAssets = Math.Max(brickAssets.Count, 2);
        int numMats = Math.Max(materials.Count, 2);
        int numPhysMats = brsVersion >= 9 ? Math.Max(physMats.Count, 2) : 0;

        for (int brickIndex = 0; brickIndex < brickCount; brickIndex++)
        {
            Brick brick = new Brick();
            brick.AssetNameIndex = bw.ReadInt(numAssets); // Asset Name Index

            brick.Size = Vector3Int.zero;
            if (bw.ReadBit())
                brick.Size = bw.ReadPositiveIntVectorPacked();

            brick.Position = bw.ReadIntVectorPacked();

            int orientation = bw.ReadInt(24);
            brick.Rotation = orientation & 3;
            brick.Direction = (orientation >> 2) % 6;

            bw.ReadBit(); // collision
            bw.ReadBit(); // visibility

            //brick.materialIndex = 1;
            if (brsVersion >= 8)
                bw.ReadInt(numMats);
            else if (bw.ReadBit())
                bw.ReadIntPacked();

            if (brsVersion >= 9)
            {
                bw.ReadInt(numPhysMats);
                bw.ReadInt(11);
            }

            if (!bw.ReadBit())
            {
                int colorIndex = bw.ReadInt(colors.Count);
                brick.Color = new ColorMode(colorIndex);
            }
            else
            {
                uint col = brsVersion >= 9 ? (uint)bw.ReadThree() : (uint)bw.ReadInt();
                int b = (int)col & 0xFF;
                int g = (int)(col >> 8) & 0xFF;
                int r = (int)(col >> 16) & 0xFF;
                int a = brsVersion >= 9 ? 255 : (int)(col >> 24) & 0xFF;
                brick.Color = new ColorMode(new Color32((byte)(Mathf.LinearToGammaSpace(r / 255f) * 255f),
                            (byte)(Mathf.LinearToGammaSpace(g / 255f) * 255f),
                            (byte)(Mathf.LinearToGammaSpace(b / 255f) * 255f),
                            (byte)(Mathf.LinearToGammaSpace(a / 255f) * 255f)));
            }
            bw.ReadIntPacked(); // owner index
            bw.ByteAlign();

            world.AddBrick(brick);
        }
#if UNITY_EDITOR
        EditorCoroutines.Execute(world.CreateGameObjects(chunksPerFrame));
#else
        StartCoroutine(world.CreateGameObjects(chunksPerFrame * 2));
#endif
    }

}

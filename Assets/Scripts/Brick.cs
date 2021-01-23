using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brick
{
    private static readonly Quaternion[] ROTATIONS = new Quaternion[4] {
        Quaternion.AngleAxis(0, Vector3.up),
        Quaternion.AngleAxis(90, Vector3.up),
        Quaternion.AngleAxis(180, Vector3.up),
        Quaternion.AngleAxis(270, Vector3.up),
    };

    private static readonly Quaternion[] DIRECTIONS = new Quaternion[6]
    {
        Quaternion.AngleAxis(90, Vector3.back),    // XPositive
        Quaternion.AngleAxis(90, Vector3.forward), // XNegative
        Quaternion.AngleAxis(90, Vector3.left),   // YPositive
        Quaternion.AngleAxis(90, Vector3.right),    // YNegative
        Quaternion.AngleAxis(0, Vector3.up),       // ZPositive
        Quaternion.AngleAxis(180, Vector3.forward) // ZNegative
    };

    private static readonly int[] BRICK_TRIS = new int[36]
    {
        // TOP
        0, 1, 2,
        2, 1, 3,
        // BOT
        4, 6, 5,
        6, 7, 5,
        // LEFT
        8, 10, 9,
        10, 11, 9,
        // RIGHT
        12, 13, 15,
        12, 15, 14,
        // FRONT
        18, 16, 17,
        18, 17, 19,
        // BACK
        20, 22, 21,
        21, 22, 23
    };

    private static readonly int[] SIDE_WEDGE_TRIS = new int[24]
    {
        // TOP
        0, 2, 1,
        // BOT
        3, 5, 4,
        // RIGHT
        6, 8, 7,
        8, 9, 7,
        // FRONT
        10, 12, 11,
        12, 13, 11,
        // SIDE
        14, 15, 16,
        15, 17, 16,
    };

    private static readonly int[] WEDGE_TRIS = new int[36]
    {
        // TOP
        0, 2, 1,
        2, 3, 1,
        // BOT
        4, 6, 5,
        6, 7, 5,
        // LEFT
        8, 10, 9,
        9, 10, 11,
        // RIGHT
        12, 13, 14,
        14, 13, 15,
        // FRONT
        16, 18, 17, 
        17, 18, 19,
        // BACK
        20, 21, 22,
        22, 21, 23,
    };

    private static readonly int[] RAMP_TRIS = new int[42]
    {
        // TOP
        0, 2, 1,
        2, 3, 1,
        // LONG
        5, 4, 6,
        6, 7, 5,
        // FRONT
        9, 8, 10,
        10, 11, 9,
        // BACK
        12, 13, 15, 
        15, 14, 12,
        // LEFT
        16, 18, 17,
        17, 18, 19,
        18, 20, 19,
        // RIGHT
        21, 22, 23,
        23, 22, 24,
        23, 24, 25,
    };

    private static readonly float SCALE = 0.05f;

    public int AssetNameIndex { get; set; }
    public Vector3Int Size { get; set; }
    public Vector3Int Position { get; set; }
    public int Rotation { get; set; }
    public int Direction { get; set; }
    public ColorMode Color { get; set; }

    public void CreateDefaultBrick(MeshData mesh)
    {
        Vector3[] verts = new Vector3[24]
        {
            // TOP
            new Vector3(- Size.x, + Size.y, - Size.z),
            new Vector3(- Size.x, + Size.y, + Size.z),
            new Vector3(+ Size.x, + Size.y, - Size.z),
            new Vector3(+ Size.x, + Size.y, + Size.z),
            // BOT
            new Vector3(- Size.x, - Size.y, - Size.z),
            new Vector3(- Size.x, - Size.y, + Size.z),
            new Vector3(+ Size.x, - Size.y, - Size.z),
            new Vector3(+ Size.x, - Size.y, + Size.z),
            // LEFT
            new Vector3(- Size.x, + Size.y, - Size.z),
            new Vector3(- Size.x, + Size.y, + Size.z),
            new Vector3(- Size.x, - Size.y, - Size.z),
            new Vector3(- Size.x, - Size.y, + Size.z),
            // RIGHT
            new Vector3(+ Size.x, + Size.y, - Size.z),
            new Vector3(+ Size.x, + Size.y, + Size.z),
            new Vector3(+ Size.x, - Size.y, - Size.z),
            new Vector3(+ Size.x, - Size.y, + Size.z),
            // FRONT
            new Vector3(- Size.x, + Size.y, - Size.z),
            new Vector3(+ Size.x, + Size.y, - Size.z),
            new Vector3(- Size.x, - Size.y, - Size.z),
            new Vector3(+ Size.x, - Size.y, - Size.z),
            // BACK
            new Vector3(- Size.x, + Size.y, + Size.z),
            new Vector3(+ Size.x, + Size.y, + Size.z),
            new Vector3(- Size.x, - Size.y, + Size.z),
            new Vector3(+ Size.x, - Size.y, + Size.z)
        };

        ApplyRotationAndColor(mesh, verts);
        AddTris(mesh, BRICK_TRIS);

        mesh.Vertices.AddRange(verts);
    }

    public void CreateSideWedge(MeshData mesh)
    {
        Vector3[] verts = new Vector3[18]
        {
            // TOP
            new Vector3(- Size.x, + Size.y, - Size.z),//0
            new Vector3(+ Size.x, + Size.y, + Size.z),//1
            new Vector3(- Size.x, + Size.y, + Size.z),//2
            // BOT
            new Vector3(- Size.x, - Size.y, - Size.z),//3
            new Vector3(- Size.x, - Size.y, + Size.z),//4
            new Vector3(+ Size.x, - Size.y, + Size.z),//5
            // Right
            new Vector3(- Size.x, + Size.y, + Size.z), //2   6
            new Vector3(+ Size.x, + Size.y, + Size.z), //1   7
            new Vector3(- Size.x, - Size.y, + Size.z), //4   8
            new Vector3(+ Size.x, - Size.y, + Size.z), //5   9
            // FRONT
            new Vector3(- Size.x, + Size.y, - Size.z),//0    10
            new Vector3(- Size.x, + Size.y, + Size.z),//2    11
            new Vector3(- Size.x, - Size.y, - Size.z),//3    12
            new Vector3(- Size.x, - Size.y, + Size.z),//4    13
            // SIDE
            new Vector3(- Size.x, + Size.y, - Size.z),//0    14
            new Vector3(+ Size.x, + Size.y, + Size.z),//1    15
            new Vector3(- Size.x, - Size.y, - Size.z),//3    16
            new Vector3(+ Size.x, - Size.y, + Size.z),//5    17
        };

        ApplyRotationAndColor(mesh, verts);
        AddTris(mesh, SIDE_WEDGE_TRIS);

        mesh.Vertices.AddRange(verts);
    }

    public void CreateWedge(MeshData mesh)
    {
        Vector3[] verts = new Vector3[24]
        {
            // TOP
            new Vector3(- Size.x, + Size.y, + Size.z),
            new Vector3(- Size.x, + Size.y, - Size.z),
            new Vector3(+ Size.x, - Size.y + 2, + Size.z),
            new Vector3(+ Size.x, - Size.y + 2, - Size.z),
            // BOT
            new Vector3(- Size.x, - Size.y, - Size.z),
            new Vector3(- Size.x, - Size.y, + Size.z),
            new Vector3(+ Size.x, - Size.y, - Size.z),
            new Vector3(+ Size.x, - Size.y, + Size.z),
            // LEFT
            new Vector3(- Size.x, - Size.y, - Size.z), //8
            new Vector3(+ Size.x, - Size.y, - Size.z), //9
            new Vector3(- Size.x, + Size.y, - Size.z), //10
            new Vector3(+ Size.x, - Size.y + 2, - Size.z), //11
            // RIGHT
            new Vector3(- Size.x, - Size.y, + Size.z), //12
            new Vector3(+ Size.x, - Size.y, + Size.z), //13
            new Vector3(- Size.x, + Size.y, + Size.z), //14
            new Vector3(+ Size.x, - Size.y + 2, + Size.z), //15
            // FRONT
            new Vector3(+ Size.x, - Size.y, - Size.z),
            new Vector3(+ Size.x, - Size.y, + Size.z),
            new Vector3(+ Size.x, - Size.y + 2, - Size.z),
            new Vector3(+ Size.x, - Size.y + 2, + Size.z),
            // BACK
            new Vector3(- Size.x, - Size.y, - Size.z),
            new Vector3(- Size.x, - Size.y, + Size.z),
            new Vector3(- Size.x, + Size.y, - Size.z),
            new Vector3(- Size.x, + Size.y, + Size.z),
        };

        ApplyRotationAndColor(mesh, verts);
        AddTris(mesh, WEDGE_TRIS);

        mesh.Vertices.AddRange(verts);
    }

    public void CreateRamp(MeshData mesh)
    {
        Vector3[] verts = new Vector3[26]
        {
            // TOP
            new Vector3(- Size.x, + Size.y, + Size.z),
            new Vector3(- Size.x, + Size.y, - Size.z),
            new Vector3(0, + Size.y, + Size.z),
            new Vector3(0, + Size.y, - Size.z),
            // LONG
            new Vector3(0, + Size.y, + Size.z),
            new Vector3(0, + Size.y, - Size.z),
            new Vector3(+ Size.x, - Size.y + 2, + Size.z),
            new Vector3(+ Size.x, - Size.y + 2, - Size.z),
            // FRONT
            new Vector3(+ Size.x, - Size.y + 2, + Size.z),
            new Vector3(+ Size.x, - Size.y + 2, - Size.z),
            new Vector3(+ Size.x, - Size.y, + Size.z),
            new Vector3(+ Size.x, - Size.y, - Size.z),
            // BACK
            new Vector3(- Size.x, - Size.y, - Size.z),
            new Vector3(- Size.x, - Size.y, + Size.z),
            new Vector3(- Size.x, + Size.y, - Size.z),
            new Vector3(- Size.x, + Size.y, + Size.z),
            // LEFT
            new Vector3(- Size.x, - Size.y, - Size.z),
            new Vector3(+ Size.x, - Size.y, - Size.z),
            new Vector3(- Size.x, + Size.y, - Size.z),
            new Vector3(+ Size.x, - Size.y + 2, - Size.z),
            new Vector3(0, + Size.y, - Size.z),
            // RIGHT
            new Vector3(- Size.x, - Size.y, + Size.z),
            new Vector3(+ Size.x, - Size.y, + Size.z),
            new Vector3(- Size.x, + Size.y, + Size.z),
            new Vector3(+ Size.x, - Size.y + 2, + Size.z),
            new Vector3(0, + Size.y, + Size.z),
        };

        ApplyRotationAndColor(mesh, verts);
        AddTris(mesh, RAMP_TRIS);
        mesh.Vertices.AddRange(verts);
    }

    public void ApplyRotationAndColor(MeshData mesh, Vector3[] verts)
    {
        // Apply scale, rotation, direction, and position to vertices
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = SCALE * verts[i];
            int rotationOffset = 0;
            switch (Direction)
            {
                case 0: rotationOffset = 2; break;
                case 2: rotationOffset = 3; break;
                case 3: rotationOffset = 1; break;
            }
            verts[i] = ROTATIONS[(Rotation + rotationOffset) % 4] * verts[i];
            verts[i] = DIRECTIONS[Direction] * verts[i];
            verts[i] = verts[i] + (Vector3)Position * SCALE;
        }

        // Apply color to vertices
        Color32[] cols = new Color32[verts.Length];
        for (int i = 0; i < cols.Length; i++)
            cols[i] = Color.index ? BRS.Instance.colors[Color.colorIndex] : Color.customColor;
        mesh.Colors.AddRange(cols);
    }

    public void AddTris(MeshData mesh, int[] triangles)
    {
        int[] tris = (int[])triangles.Clone();
        for (int i = 0; i < tris.Length; ++i)
            tris[i] = tris[i] + mesh.Vertices.Count;
        mesh.Triangles.AddRange(tris);
    }

    public void CreateBrick(MeshData mesh, string assetName)
    {
        GameObject gameObject = GameObject.Find(assetName);
        if (gameObject == null)
            return;

        Mesh brickMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] verts = (Vector3[])brickMesh.vertices.Clone();

        ApplyRotationAndColor(mesh, verts);
        AddTris(mesh, (int[])brickMesh.triangles.Clone());
        mesh.Vertices.AddRange(verts);
    }

    public void CreateMesh(MeshData mesh)
    {
        string assetName = BRS.Instance.brickAssets[AssetNameIndex];

        if (assetName == "PB_DefaultBrick" || assetName == "PB_DefaultTile" || assetName == "PB_DefaultMicroBrick")
            CreateDefaultBrick(mesh);
        else if (assetName == "PB_DefaultMicroWedge" || assetName == "PB_DefaultSideWedge" || assetName == "PB_DefaultSideWedgeTile")
            CreateSideWedge(mesh);
        else if (assetName == "PB_DefaultWedge")
            CreateWedge(mesh);
        else if (assetName == "PB_DefaultRamp")
            CreateRamp(mesh);
        else
            CreateBrick(mesh, assetName);
    }

}

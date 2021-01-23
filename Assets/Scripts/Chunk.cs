using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public List<Brick> bricks;

    public Chunk()
    {
        bricks = new List<Brick>();
    }

    public void AddBrick(Brick brick)
    {
        bricks.Add(brick); 
    }

    public void CreateGameObject(Transform parent, Vector3Int chunkLocation)
    {
        GameObject gameObject = new GameObject("Chunk " + chunkLocation, typeof(MeshFilter), typeof(MeshRenderer));
        gameObject.GetComponent<MeshRenderer>().material = BRS.Instance.baseMat;
        gameObject.transform.parent = parent;

        MeshData meshData = new MeshData();

        foreach (Brick brick in bricks)
        {
            brick.CreateMesh(meshData);
        }

        Mesh mesh = new Mesh()
        {
            vertices = meshData.Vertices.ToArray(),
            triangles = meshData.Triangles.ToArray(),
            colors32 = meshData.Colors.ToArray()
        };

        mesh.RecalculateNormals();
        mesh.Optimize();

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        //Debug.Log(meshFilter.sharedMesh.vertices.Length + ", " + meshFilter.sharedMesh.triangles.Length + ", " + meshFilter.sharedMesh.colors32.Length);

        if (BRS.Instance.generateColliders)
        {
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
    }

    /*
    public void CreateEntity()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld)
            );

        Entity chunkEntity = entityManager.CreateEntity(archetype);
        entityManager.AddComponentData(chunkEntity, new Translation
        {
            Value = new float3(0f, 0f, 0f)
        });

        MeshData meshData = new MeshData();
        foreach (Brick brick in bricks)
            brick.CreateMesh(meshData);
        Mesh chunkMesh = new Mesh()
        {
            vertices = meshData.Vertices.ToArray(),
            triangles = meshData.Triangles.ToArray(),
            colors32 = meshData.Colors.ToArray()
        };
        chunkMesh.RecalculateNormals();
        chunkMesh.Optimize();

        entityManager.AddSharedComponentData(chunkEntity, new RenderMesh
        {
            mesh = chunkMesh,
            material = BRS.Instance.baseMat
        });
    }*/

}

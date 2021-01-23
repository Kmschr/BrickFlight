using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickWorld
{

    private readonly Dictionary<Vector3Int, Chunk> chunks;

    public BrickWorld()
    {
        chunks = new Dictionary<Vector3Int, Chunk>();
    }

    public IEnumerator CreateGameObjects(int chunksPerFrame)
    {
        int i = 0;
        GameObject parent = new GameObject("Bricks");
        foreach (KeyValuePair<Vector3Int, Chunk> entry in chunks)
        {
            entry.Value.CreateGameObject(parent.transform, entry.Key);
            i++;
            if (i % chunksPerFrame == 0)
                yield return null;
        }
    }

    public void AddBrick(Brick brick)
    {
        Vector3Int chunkKey = brick.Position / 320;
        if (!chunks.ContainsKey(chunkKey))
        {
            chunks.Add(chunkKey, new Chunk());
        }
        chunks[chunkKey].AddBrick(brick);
    }

}

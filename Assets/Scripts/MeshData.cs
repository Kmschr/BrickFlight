using UnityEngine;
using System.Collections.Generic;

public class MeshData 
{
    public List<Vector3> Vertices { get; set; }
    public List<int> Triangles { get; set; }
    public List<Color32> Colors { get; set; }

    public MeshData()
    {
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
        Colors = new List<Color32>();
    }

/*    public Mesh BuildMesh()
    {
        List<int> trianglesToDraw = new List<int>();

        int numFacesCulled = 0;

        // Keep track of the first triangle index for each face
        int currentTriangleIndex = 0;

        // For every face in the mesh data
        for (int currentFaceIndex = 0; currentFaceIndex < NumTrisInFace.Count; ++currentFaceIndex)
        {
            // Build a set of the vertices in this face
            HashSet<Vector3> verticesInCurrentFace = new HashSet<Vector3>();
            for (int triangleIndex = 0; triangleIndex < NumTrisInFace[currentFaceIndex]; ++triangleIndex)
                for (int i = 0; i < 3; ++i)
                    verticesInCurrentFace.Add(Vertices[Triangles[currentTriangleIndex + triangleIndex * 3 + i]]);

            // Keep track of is this a face we want to draw or not
            bool isCulled = false;

            // Compare the set of vertices in this face to the set of vertices in the other faces
            int otherTriangleIndex = 0;
            for (int otherFaceIndex = 0; otherFaceIndex < NumTrisInFace.Count; ++otherFaceIndex)
            {
                // Dont compare the same face against itself
                if (currentFaceIndex == otherFaceIndex)
                {
                    otherTriangleIndex += NumTrisInFace[otherFaceIndex] * 3;
                    continue;
                }

                // Build a set of the vertices in the other face
                HashSet<Vector3> verticesInOtherFace = new HashSet<Vector3>();
                for (int triangleIndex = 0; triangleIndex < NumTrisInFace[otherFaceIndex]; ++triangleIndex)
                    for (int i = 0; i < 3; ++i)
                        verticesInOtherFace.Add(Vertices[Triangles[otherTriangleIndex + triangleIndex * 3 + i]]);

                // Check if we have at least 3 vertices in common for the face
                int numCommonVertices = 0;
                foreach (Vector3 vertex in verticesInCurrentFace)
                    if (verticesInOtherFace.Contains(vertex))
                        numCommonVertices++;

                // Face is on top of the other face and is the same size as the other face, so we will cull it
                if (numCommonVertices >= 3)
                {
                    if (verticesInCurrentFace.Count == verticesInOtherFace.Count)
                    {
                        isCulled = true;
                        numFacesCulled++;
                        break;
                    }
                }

                otherTriangleIndex += NumTrisInFace[otherFaceIndex] * 3;
            }

            if (!isCulled)
            {
                for (int triangleIndex = 0; triangleIndex < NumTrisInFace[currentFaceIndex]; ++triangleIndex)
                    for (int i = 0; i < 3; i++)
                        trianglesToDraw.Add(Triangles[currentTriangleIndex + triangleIndex * 3 + i]);
            }

            // Keep track of the first triangle index for each face
            currentTriangleIndex += NumTrisInFace[currentFaceIndex] * 3;
        }

        Mesh mesh = new Mesh()
        {
            vertices = Vertices.ToArray(),
            triangles = trianglesToDraw.ToArray(),
            colors32 = Colors.ToArray()
        };

        Debug.Log(numFacesCulled);

        return mesh;
    }*/

}

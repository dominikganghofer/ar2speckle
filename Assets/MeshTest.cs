using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTest : MonoBehaviour
{
    [ContextMenu("Test Mesh")]
    public void TestMesh()
    {
        int vertexCount = 1000;
        Vector3[] vertices = new Vector3[vertexCount];

        int[] indices = new int[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i] =Random.insideUnitSphere;
            indices[i] = i;
        }

        Mesh m = new Mesh();
        m.vertices = vertices;
        m.SetIndices(indices, MeshTopology.Points, 0);
        m.RecalculateBounds();

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);

        m.colors = colors;

        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = m;
        Debug.Log(m.bounds);
        // assign the array of colors to the Mesh.
    }

}
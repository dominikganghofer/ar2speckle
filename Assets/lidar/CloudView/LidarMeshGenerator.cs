using System.Linq;
using UnityEngine;

namespace ar2gh.lidar
{
    public static class LidarMeshGenerator
    {
        public static (int[] indices, Color[] colors, Vector3[] verts) Generate(LidarPoint[] cloud)
        {
            var validVrts = cloud.Where(c => c.IsValid).ToArray();
            var count = validVrts.Count();

            // var vertexCountHasChanged = _mesh.vertices.Length != count;
            //if (!vertexCountHasChanged)
            //   return;

            var indices = new int[count];
            for (var i = 0; i < count; i++)
            {
                indices[i] = i;
            }

            var verts = validVrts.Select(c => c.WorldPosition).ToArray();
            var colors = validVrts
                .Select(c => new Color(c.Color.r / 255f, c.Color.g / 255f, c.Color.b / 255f))
                .ToArray();
            return (indices, colors, verts);
        }

        public static Mesh GenerateMesh((int[] indices, Color[] colors, Vector3[] verts) meshData)
        {
            var mesh = new Mesh {indexFormat = UnityEngine.Rendering.IndexFormat.UInt32};
            mesh.Clear();
            mesh.vertices = meshData.verts;
            mesh.colors = meshData.colors;
            mesh.SetIndices(meshData.indices, MeshTopology.Points, 0);
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
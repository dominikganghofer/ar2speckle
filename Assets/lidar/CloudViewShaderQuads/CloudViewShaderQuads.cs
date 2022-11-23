using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ar2gh.lidar
{
    /// <summary>
    /// Renders  <see cref="LidarPoint"/>s as a mesh with vertex colors.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class CloudViewShaderQuads : MonoBehaviour
    {
        private Mesh _mesh;

        public void RenderVertices(LidarPoint[] cloud)
        {
            if (_mesh == null)
            {
                // generate mesh
                _mesh = new Mesh {indexFormat = UnityEngine.Rendering.IndexFormat.UInt32};
                gameObject.GetComponent<MeshFilter>().mesh = _mesh;
            }

            var validVrts = cloud.Where(c => c.IsValid).ToArray();
            var count = validVrts.Count();

            // var vertexCountHasChanged = _mesh.vertices.Length != count;
            //if (!vertexCountHasChanged)
            //   return;

            var indices = new int[count * 2 * 3];
            for (var i = 0; i < count; i++)
            {
                indices[i * 6 + 0] = i * 4 + 0;
                indices[i * 6 + 1] = i * 4 + 1;
                indices[i * 6 + 2] = i * 4 + 2;

                indices[i * 6 + 3] = i * 4 + 0;
                indices[i * 6 + 4] = i * 4 + 2;
                indices[i * 6 + 5] = i * 4 + 3;
            }

            _mesh.Clear();
            var centers = validVrts.Select(c => c.WorldPosition).ToArray();
            var centerColors = validVrts
                .Select(c => new Color(c.Color.r / 255f, c.Color.g / 255f, c.Color.b / 255f))
                .ToArray();

            var vrts = new Vector3[count * 4];
            var colors = new Color[count * 4];

            var offset = 0.001f;
            for (int i = 0; i < count * 4; i++)
            {
                vrts[i * 4 + 0] = centers[i] + new Vector3(offset, offset, 0);
                vrts[i * 4 + 1] = centers[i] + new Vector3(-offset, offset, 0);
                vrts[i * 4 + 2] = centers[i] + new Vector3(offset, -offset, 0);
                vrts[i * 4 + 3] = centers[i] + new Vector3(-offset, -offset, 0);
                colors[i * 4 + 0] = centerColors[i];
                colors[i * 4 + 1] = centerColors[i];
                colors[i * 4 + 2] = centerColors[i];
                colors[i * 4 + 3] = centerColors[i];
            }


            _mesh.vertices = vrts;
            _mesh.colors = colors;
            _mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            _mesh.RecalculateBounds();
        }
    }
}
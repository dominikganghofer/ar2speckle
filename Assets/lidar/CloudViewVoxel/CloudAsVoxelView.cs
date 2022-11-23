using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ar2gh.lidar;
using UnityEngine;

public class CloudAsVoxelView : MonoBehaviour
{
    private Mesh _mesh;

        public  void RenderVertices(LidarPoint[] cloud)
        {
            if (_mesh == null)
            {
                // generate mesh
                _mesh = new Mesh {indexFormat = UnityEngine.Rendering.IndexFormat.UInt32};
                gameObject.GetComponent<MeshFilter>().mesh = _mesh;
            }

            var validVrts = cloud.Where(c => c.IsValid).ToArray();
            var pointCount = validVrts.Count();

            // var vertexCountHasChanged = _mesh.vertices.Length != count;
            //if (!vertexCountHasChanged)
            //   return;

            //    4--5
            // 4--0--1--5
            // 6--2--3--7
            //    6--7

            // tris per vertex: 6 * 2 
            // newVertices = vertexCount * 8
            // indices = vertexCount * 6 * 4

            var indices = new int[pointCount * 6 * 2 * 3];
            for (var i = 0; i < pointCount; i++)
            {
                var currentQuadPosition = i * 6;
                var currentNewVertexPosition = i * 8;

                //0123
                indices[currentQuadPosition + 0] = currentNewVertexPosition + 0;
                indices[currentQuadPosition + 1] = currentNewVertexPosition + 3;
                indices[currentQuadPosition + 2] = currentNewVertexPosition + 1;
                
                indices[currentQuadPosition + 3] = currentNewVertexPosition + 0;
                indices[currentQuadPosition + 4] = currentNewVertexPosition + 2;
                indices[currentQuadPosition + 5] = currentNewVertexPosition + 3;

                //0145
                indices[currentQuadPosition + 6] = currentNewVertexPosition + 0;
                indices[currentQuadPosition + 7] = currentNewVertexPosition + 5;
                indices[currentQuadPosition + 8] = currentNewVertexPosition + 4;

                indices[currentQuadPosition + 9] = currentNewVertexPosition + 0;
                indices[currentQuadPosition + 10] = currentNewVertexPosition + 1;
                indices[currentQuadPosition + 11] = currentNewVertexPosition + 5;

                //2367
                indices[currentQuadPosition + 12] = currentNewVertexPosition + 2;
                indices[currentQuadPosition + 13] = currentNewVertexPosition + 7;
                indices[currentQuadPosition + 14] = currentNewVertexPosition + 3;
                
                indices[currentQuadPosition + 15] = currentNewVertexPosition + 2;
                indices[currentQuadPosition + 16] = currentNewVertexPosition + 6;
                indices[currentQuadPosition + 17] = currentNewVertexPosition + 7;
                
                //2367
                indices[currentQuadPosition + 18] = currentNewVertexPosition + 0;
                indices[currentQuadPosition + 19] = currentNewVertexPosition + 6;
                indices[currentQuadPosition + 20] = currentNewVertexPosition + 2;
                
                indices[currentQuadPosition + 21] = currentNewVertexPosition + 0;
                indices[currentQuadPosition + 22] = currentNewVertexPosition + 4;
                indices[currentQuadPosition + 23] = currentNewVertexPosition + 6;
                
                //1537
                indices[currentQuadPosition + 24] = currentNewVertexPosition + 1;
                indices[currentQuadPosition + 25] = currentNewVertexPosition + 7;
                indices[currentQuadPosition + 26] = currentNewVertexPosition + 5;
                
                indices[currentQuadPosition + 27] = currentNewVertexPosition + 1;
                indices[currentQuadPosition + 28] = currentNewVertexPosition + 3;
                indices[currentQuadPosition + 29] = currentNewVertexPosition + 7;
                
                //4567
                indices[currentQuadPosition + 30] = currentNewVertexPosition + 4;
                indices[currentQuadPosition + 31] = currentNewVertexPosition + 7;
                indices[currentQuadPosition + 32] = currentNewVertexPosition + 6;
                
                indices[currentQuadPosition + 33] = currentNewVertexPosition + 4;
                indices[currentQuadPosition + 34] = currentNewVertexPosition + 5;
                indices[currentQuadPosition + 35] = currentNewVertexPosition + 7;
            }

            _mesh.Clear();
            var centerPoints = validVrts.Select(c => c.WorldPosition).ToArray();
            var centerColors = validVrts
                .Select(c => new Color(c.Color.r / 255f, c.Color.g / 255f, c.Color.b / 255f))
                .ToArray();

            var verts = new Vector3[pointCount * 8];
            var colors = new Color[pointCount * 8];

            var a = 0.001f; //offset
            var offsets = new []
            {
                new Vector3(-a, -a, -a),
                new Vector3(a, -a, -a),
                new Vector3(-a, a, -a),
                new Vector3(a, a, -a),
                new Vector3(-a, -a, a),
                new Vector3(a, -a, a),
                new Vector3(a, a, a),
                new Vector3(a, a, a),
            };

            for (var i = 0; i < pointCount; i++)
            {
                for (var cubeCornerIndex = 0; cubeCornerIndex < 8; cubeCornerIndex++)
                {
                    verts[i * 8 + cubeCornerIndex] = centerPoints[i] + offsets[cubeCornerIndex];
                    colors[i * 8 + cubeCornerIndex] = centerColors[i];
                }
            }

            _mesh.SetVertices(verts);
            _mesh.SetColors(colors);
            _mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            _mesh.RecalculateBounds();
        }
}

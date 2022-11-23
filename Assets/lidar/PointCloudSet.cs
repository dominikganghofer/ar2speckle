using System;
using System.Collections.Generic;
using System.Linq;
using ar2gh.lidar;
using UnityEngine;

namespace lidar
{
    public class PointCloudSet
    {
        private Dictionary<(long x, long y, long z), Color> _pointCloud = new();

        public PointCloudSet()
        {
        }


        public void AddCloud(LidarPoint[] cloud)
        {
            var validVertices = cloud.Where(c => c.IsValid).ToArray();

            foreach (var lidarPoint in cloud)
            {
                var x = FloatToCMLong(lidarPoint.WorldPosition.x);
                var y = FloatToCMLong(lidarPoint.WorldPosition.y);
                var z = FloatToCMLong(lidarPoint.WorldPosition.z);
                var key = (x, y, z);

                if (_pointCloud.ContainsKey(key))
                    _pointCloud[key] = Color.Lerp(_pointCloud[key], lidarPoint.Color, 0.5f);
                else
                    _pointCloud[key] = lidarPoint.Color;
            }

            // .Select(c => new Color(c.Color.r / 255f, c.Color.g / 255f, c.Color.b / 255f))
        }


        private (long x, long y, long z) VectorToCMTuple(Vector3 v) =>
            (FloatToCMLong(v.x), FloatToCMLong(v.y), FloatToCMLong(v.z));

        private Vector3 CMTupleToVector((long x, long y, long z) tuple) => new(CMLongToFloat(tuple.x),
            CMLongToFloat(tuple.y), CMLongToFloat(tuple.z));

        private static long FloatToCMLong(float p) => Mathf.RoundToInt(p * 500);
        private static float CMLongToFloat(long cm) => cm * 0.002f;

        public Mesh GetMesh()
        {
            var mesh = new Mesh
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
                vertices = _pointCloud.Select(kvp => CMTupleToVector(kvp.Key)).ToArray(),
                colors = _pointCloud.Select(kvp => kvp.Value).ToArray()
            };
            mesh.SetIndices(Enumerable.Range(0, _pointCloud.Count).ToArray(), MeshTopology.Points, 0);
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
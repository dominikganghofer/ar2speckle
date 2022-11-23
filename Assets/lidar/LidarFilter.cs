using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ar2gh.lidar
{
    public class LidarFilter
    {
        private static HashSet<Vector3Int> _sentPositions = new HashSet<Vector3Int>();
        private static LidarPointComparer _pointComparer = new LidarPointComparer();

        public LidarPoint[] FilterInvalidPoints(LidarPoint[] cloud)
        {
            var news = cloud.Distinct(_pointComparer)
                .Where(cp => !_sentPositions.Contains(cp.SnappedPositionWorld)).ToArray();
            _sentPositions.UnionWith(news.Select(cp => cp.SnappedPositionWorld));
            Debug.Log($"{news.Length} / {cloud.Length} points are left after filter.");
            return news;
        }

        /// <summary>
        /// Two <see cref="LidarPoint"/>s are equal if they have the same snapped position.
        /// </summary>
        private class LidarPointComparer : IEqualityComparer<LidarPoint>
        {
            public bool Equals(LidarPoint x, LidarPoint y)
            {
                return x.SnappedPositionWorld == y.SnappedPositionWorld;
            }

            public int GetHashCode(LidarPoint obj)
            {
                return obj.SnappedPositionWorld.GetHashCode();
            }
        }
    }
}
using System;
using UnityEngine;

namespace ar2gh.lidar
{
    /// <summary>
    /// Item of the Lidar Point Cloud
    /// </summary>
    public struct LidarPoint
    {
        public Vector3Int SnappedPositionWorld;
        public Vector3 PositionCameraSpace;
        public Color32 Color;
        public bool IsValid;
        public Vector3 WorldPosition;
    }
}
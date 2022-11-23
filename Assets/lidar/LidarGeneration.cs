using ar2gh.camera;
using ar2gh.lidar;
using ar2gh.mesh;
using lidar;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace ar2gh
{
    /// <summary>
    /// Main Controller. Registers at ARKit components and sends updates to Grasshopper via <see cref="UDPConnection"/>.
    /// Has a toggle to decide if the lidar point cloud or the environment mesh should be send. 
    /// </summary>
    public class LidarGeneration : MonoBehaviour
    {
        [SerializeField] private LidarPointCloud _lidarPointCloud = null;
        [SerializeField] private LidarPointCloudView _lidarPointCloudView = null;

        private void Start()
        {
            _lidarPointCloud.CloudUpdateEvent += SendLidarPointCloudUpdate;
        }

        private bool _busy;
        
        void Update()
        {
            if (_busy && !Input.GetMouseButtonDown(0)) 
                return;
            Debug.Log("Request");
            _lidarPointCloud.RequestNextCloud();
            _busy = true;
        }

        private PointCloudSet _cloudSet = new();
        private void SendLidarPointCloudUpdate(LidarPoint[] cloud)
        {
            _cloudSet.AddCloud(cloud);
            var mesh = _cloudSet.GetMesh();
            _lidarPointCloudView.RenderVertices(mesh);
            _busy = false;
        }
    }
}
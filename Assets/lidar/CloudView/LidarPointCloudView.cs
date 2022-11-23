using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ar2gh.lidar
{
    /// <summary>
    /// Renders  <see cref="LidarPoint"/>s as a mesh with vertex colors.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class LidarPointCloudView : AbstractCloudView
    {
        private Mesh _mesh;

        // [Range(0.05f, 1f)] public float FadeInSpeed;
        // private bool _isFadedIn = false;
        private Material _material;

        public void Update()
        {
            // if (_isFadedIn || _material == null)
            //     return;
            //
            // var currentFadeIn = _material.GetFloat("FadeIn");
            // var newFadeIn = Mathf.Lerp(currentFadeIn, 1f, 0.5f*(1+FadeInSpeed));
            // if (newFadeIn > 0.99f)
            // {
            //     newFadeIn = 1f;
            //     _isFadedIn = true;
            // }
            //
            // _material.SetFloat("FadeIn", newFadeIn);
        }

        public override void RenderVertices(Mesh mesh)
        {
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
            _material = gameObject.GetComponent<MeshRenderer>().material;
            _material.SetFloat("FadeIn", 1f);

        }
    }
}
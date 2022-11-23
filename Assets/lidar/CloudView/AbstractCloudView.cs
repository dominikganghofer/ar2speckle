using System.Collections;
using System.Collections.Generic;
using ar2gh.lidar;
using UnityEngine;

public abstract class AbstractCloudView : MonoBehaviour
{
    public abstract void RenderVertices(Mesh cloud);
}
using System;
using Game.Utility;
using UnityEngine;

namespace Game.Scene
{
    public class CameraSpawningPoint: UniqueSceneElement<CameraSpawningPoint>
    {
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DebugExtension.DrawWireSphere(transform.position, Color.green);
        }
#endif
    }
}
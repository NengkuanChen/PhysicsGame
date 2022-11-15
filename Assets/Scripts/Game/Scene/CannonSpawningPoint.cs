using System;
using Blobcreate.ProjectileToolkit;
using Game.Utility;
using UnityEngine;

namespace Game.Scene
{
    public class CannonSpawningPoint: UniqueSceneElement<CannonSpawningPoint>
    {


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DebugExtension.DrawWireSphere(transform.position, Color.red);
        }
#endif
    }
}
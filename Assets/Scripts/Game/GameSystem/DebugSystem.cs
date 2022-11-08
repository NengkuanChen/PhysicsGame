using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Game.GameSystem
{
    public class DebugSystem : SystemBase
    {
        private static int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EditorApplication.isPaused = true;
            }
        }


    }
}
#endif
#if UNITY_EDITOR
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Utility
{
    public static class RuntimeEditorUtility
    {
        public static void CheckIsInPrefabStageOrThrErr()
        {
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                throw new Exception("当前没有在预制模式中");
            }
        }

        public static bool CheckIsInGameObjectPrefabStage(GameObject checkGameObject)
        {
            if (checkGameObject == null)
            {
                return false;
            }

            var currentPrefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            return currentPrefabStage != null && currentPrefabStage.prefabContentsRoot == checkGameObject;
        }

        public static void CheckIsInGameObjectPrefabStageOrThrErr(GameObject checkGameObject)
        {
            if (!CheckIsInGameObjectPrefabStage(checkGameObject))
            {
                throw new Exception($"物体{checkGameObject.name}没有处在它的预制模式中");
            }
        }

        public static bool IsInPrefabStage()
        {
            return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        public static void CheckMeshColliderAndEnableConvex(IList<Collider> colliders)
        {
            Assert.IsNotNull(colliders);

            foreach (var collider in colliders)
            {
                if (collider is MeshCollider meshCollider)
                {
                    meshCollider.convex = true;
                }
            }
        }
    }
}
#endif
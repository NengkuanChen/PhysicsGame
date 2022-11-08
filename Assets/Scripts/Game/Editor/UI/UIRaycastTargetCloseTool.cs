using Game.Utility;
using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

namespace Game.Editor.UI
{
    public static class UIRaycastTargetCloseTool
    {
        [MenuItem("Tools/UI/关闭当前UI预制所有的RaycastTarget")]
        private static void CloseAllRaycastTarget()
        {
            RuntimeEditorUtility.CheckIsInPrefabStageOrThrErr();

            var root = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;

            var allGraphics = root.GetComponentsInChildren<Graphic>(true);
            foreach (var graphic in allGraphics)
            {
                // Log.Info(graphic.name);
                var button = graphic.GetComponent<Button>();
                if (button != null)
                {
                    continue;
                }

                var scrollRect = graphic.GetComponent<ScrollRect>();
                if (scrollRect != null)
                {
                    continue;
                }

                if (graphic is EmptyButton emptyButton)
                {
                    continue;
                }

                if (graphic.raycastTarget)
                {
                    graphic.raycastTarget = false;
                    Debug.Log($"取消{graphic.name} 的 raycast target");
                }
            }
        }
    }
}
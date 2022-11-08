using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Editor.AssetProcess
{
    [CreateAssetMenu(fileName = "MeshReadableProcessSetting",
        menuName = "EditorSettings/MeshReadableProcessSetting",
        order = 0)]
    public class MeshReadableProcessSetting : ScriptableObject
    {
        [SerializeField, Required, LabelText("引用mesh资源目录"), FolderPath]
        private string[] rootAssetPaths;
        public string[] RootAssetPaths => rootAssetPaths;
    }
}
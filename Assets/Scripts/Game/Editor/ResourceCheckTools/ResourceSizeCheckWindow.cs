using System;
using System.Collections.Generic;
using Game.Editor.AssetBundle;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityGameFramework.Editor.ResourceTools;
using Object = UnityEngine.Object;

namespace Game.Editor.ResourceCheckTools
{
    /// <summary>
    /// 检查资源大小
    /// </summary>
    public class ResourceSizeCheckWindow : OdinEditorWindow
    {
        [MenuItem("资源检查/资源大小")]
        private static void ShowWindow()
        {
            var window = GetWindow<ResourceSizeCheckWindow>();
            window.titleContent = new GUIContent("资源检查");
            window.minSize = new Vector2(600, 600);
            window.Show();
        }

        [Serializable]
        private class AssetSizeInfo
        {
            private string assetPath;
            [HorizontalGroup(), ShowInInspector, ReadOnly, HideLabel]
            private Object asset;
            private long size;
            public long Size => size;
            [HorizontalGroup(), ShowInInspector, ReadOnly, HideLabel]
            private string sizeString;

            public AssetSizeInfo(string assetPath)
            {
                this.assetPath = assetPath;
                asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                var allSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                if (allSubAssets.Length > 0)
                {
                    foreach (var subAsset in allSubAssets)
                    {
                        size += Profiler.GetRuntimeMemorySizeLong(subAsset);
                    }
                }
                else
                {
                    size = Profiler.GetRuntimeMemorySizeLong(asset);
                }

                sizeString = UnityEditor.EditorUtility.FormatBytes(size);
            }
        }

        [SerializeField, LabelText("资源信息"), InfoBox("显示的是载入到内存之后的大小，仅供参考"),
         ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, Expanded = true)]
        private List<AssetSizeInfo> assetSizeInfos = new List<AssetSizeInfo>();

        [Button("开始检查", ButtonSizes.Large)]
        private void CheckResources()
        {
            assetSizeInfos.Clear();
            AssetBundleDeployer.AutoAssignAssetBundles();
            var resourceCollection = new ResourceCollection();
            if (!resourceCollection.Load())
            {
                Debug.Log("Load asset bundle collection failure");
                return;
            }

            var allAssetInfos = resourceCollection.GetAssets();
            foreach (var assetInfo in allAssetInfos)
            {
                assetSizeInfos.Add(new AssetSizeInfo(assetInfo.Name));
            }

            assetSizeInfos.Sort((a, b) => (int) (b.Size - a.Size));
        }
    }
}
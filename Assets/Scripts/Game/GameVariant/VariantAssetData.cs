using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using StringExtensionLibrary;
using UnityEditor;
using UnityEngine;

namespace Game.GameVariant
{
    [CreateAssetMenu(fileName = "VariantAssetData",
        menuName = "Settings/GameVariant/VariantAssetData(不要手动创建)",
        order = 1)]
    public class VariantAssetData : ScriptableObject
    {
        [SerializeField, LabelText("资源目录名"), ReadOnly]
        public string rootFolderName;
        [SerializeField, LabelText("变体资源路径"), ReadOnly]
        private List<string> variantAssetPaths;

        private HashSet<string> variantAssetPathSet;

        public void Init()
        {
            if (variantAssetPaths == null || variantAssetPaths.Count == 0)
            {
                return;
            }

            variantAssetPathSet = new HashSet<string>();
            foreach (var path in variantAssetPaths)
            {
                variantAssetPathSet.Add(path);
            }

        #if !UNITY_EDITOR
            //运行时节约一点点内存
            variantAssetPaths = null;
        #endif
        }

        public bool ContainAsset(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return false;
            }

            return variantAssetPathSet.Contains(relativePath);
        }

    #if UNITY_EDITOR
        [Button(ButtonSizes.Large, Name = "搜索变体资源")]
        public void Editor_ScanVariantAssets()
        {
            //没有目录,就是默认资源
            if (string.IsNullOrEmpty(rootFolderName))
            {
                variantAssetPaths = null;
                return;
            }

            variantAssetPaths ??= new List<string>();
            variantAssetPaths.Clear();
            variantAssetPaths.AddRange(ScanAssetsInVariantFolder());
        }

        private List<string> ScanAssetsInVariantFolder()
        {
            var result = new List<string>();
            var rootPath = $"Assets/Game/Variants/{rootFolderName}";
            var assetGuids = AssetDatabase.FindAssets("", new[] {rootPath});
            foreach (var guid in assetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var relativePath = assetPath.RemovePrefix($"{rootPath}/");
                var fileName = assetPath.Remove(0, assetPath.LastIndexOf("/", StringComparison.Ordinal) + 1);

                //排除掉自己
                if (fileName == "VariantAssetData.asset")
                {
                    continue;
                }

                //没有后缀名排除掉
                var haveExtension = fileName.Contains(".");
                if (!haveExtension)
                {
                    continue;
                }

                result.Add(relativePath);
            }

            return result;
        }

        // [Button(ButtonSizes.Large, Name = "检查不一致资源")]
        public List<string> Editor_DetectDifferentAssets()
        {
            var differentAssetPaths = new List<string>();
            var assetsRelativePathInVariantFolder = ScanAssetsInVariantFolder();
            foreach (var relativePathInVariantFolder in assetsRelativePathInVariantFolder)
            {
                var pathInDefaultFolder = $"{Application.dataPath}/Game/{relativePathInVariantFolder}";
                if (!File.Exists(pathInDefaultFolder))
                {
                    differentAssetPaths.Add(relativePathInVariantFolder);
                }
            }

            return differentAssetPaths;
        }
    #endif
    }
}
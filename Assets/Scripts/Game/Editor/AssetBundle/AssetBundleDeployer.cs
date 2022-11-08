using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace Game.Editor.AssetBundle
{
    /// <summary>
    /// 自动排布asset bundle内容
    /// </summary>
    public static class AssetBundleDeployer
    {
        /// <summary>
        /// 属于该目录及其子目录的，全部打到一个ab里面
        /// </summary>
        private static readonly string[] assetBundleRootPaths = {"Setting", "Shader", "UI/Form", "Entity/Road"};

        private const string GameResourceAssetPathRoot = "Assets/Game/GameResources";

        private static Dictionary<List<string>, int> uniqueIdDic = new Dictionary<List<string>, int>();

        [MenuItem("Game Framework/Resource Tools/Auto Assign AssetBundles")]
        public static void AutoAssignAssetBundles()
        {
            uniqueIdDic.Clear();
            var sourceAssetList = GetAllSourceAssets();
            var assetHostRecorders = FindAssetHost(sourceAssetList);
            AssignAssetBundles(sourceAssetList, assetHostRecorders);
        }

        private static List<SourceAsset> GetAllSourceAssets()
        {
            string[] sourceAssetSearchPaths = {"Assets/Game"};
            var SourceAssetUnionTypeFilter = "t:Scene t:Prefab t:Shader t:Model t:Material t:Texture t:AudioClip " +
                                             "t:AnimationClip t:AnimatorController t:Font t:TextAsset t:ScriptableObject " +
                                             "t:shadervariantcollection t:physicMaterial";
            var SourceAssetUnionLabelFilter = "l:AssetBundleInclusive";
            var SourceAssetExceptTypeFilter = "t:Script";
            var SourceAssetExceptLabelFilter = "l:AssetBundleExclusive";

            var tempGuids = new HashSet<string>();
            tempGuids.UnionWith(AssetDatabase.FindAssets(SourceAssetUnionTypeFilter, sourceAssetSearchPaths));
            tempGuids.UnionWith(AssetDatabase.FindAssets(SourceAssetUnionLabelFilter, sourceAssetSearchPaths));
            tempGuids.ExceptWith(AssetDatabase.FindAssets(SourceAssetExceptTypeFilter, sourceAssetSearchPaths));
            tempGuids.ExceptWith(AssetDatabase.FindAssets(SourceAssetExceptLabelFilter, sourceAssetSearchPaths));

            Debug.Log($"find asset count: {tempGuids.Count}");

            var sourceAssetList = new List<SourceAsset>();
            var sourceAssetRootPath = "Assets/Game";
            var sourceAssetRoot = new SourceFolder(sourceAssetRootPath, null);

            var assetGuids = new List<string>(tempGuids).ToArray();
            foreach (var assetGuid in assetGuids)
            {
                var fullPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                // Debug.Log(fullPath);
                if (AssetDatabase.IsValidFolder(fullPath))
                {
                    // Skip folder.
                    continue;
                }

                var assetPath = fullPath.Substring(sourceAssetRootPath.Length + 1);
//            Debug.Log($"asset path {assetPath}");
                var splitPath = assetPath.Split('/');
                var folder = sourceAssetRoot;
                for (var i = 0; i < splitPath.Length - 1; i++)
                {
                    var subFolder = folder.GetFolder(splitPath[i]);
                    folder = subFolder ?? folder.AddFolder(splitPath[i]);
                }

                var asset = folder.AddAsset(assetGuid, fullPath, splitPath[splitPath.Length - 1]);
                sourceAssetList.Add(asset);
            }

            return sourceAssetList;
        }

        private class AssetHostRecorder
        {
            public SourceAsset sourceAsset;
            public List<string> hostAssetGuids = new List<string>();

            public void Sort()
            {
                hostAssetGuids.Sort();
            }
        }

        private static List<AssetHostRecorder> FindAssetHost(List<SourceAsset> sourceAssetList)
        {
            var assetHostRecordersMap = new Dictionary<string, AssetHostRecorder>();
            foreach (var sourceAsset in sourceAssetList)
            {
                if (sourceAsset.Path.StartsWith(GameResourceAssetPathRoot))
                {
                    assetHostRecordersMap.Add(sourceAsset.Guid, new AssetHostRecorder {sourceAsset = sourceAsset});
                }
            }

            for (var i = 0; i < sourceAssetList.Count; i++)
            {
                var sourceAsset = sourceAssetList[i];
                if (!sourceAsset.Path.StartsWith(GameResourceAssetPathRoot))
                {
                    UnityEditor.EditorUtility.DisplayProgressBar("分析资源依赖",
                        sourceAsset.Path,
                        (float) i / sourceAssetList.Count);
                    var dependencies = AssetDatabase.GetDependencies(sourceAsset.Path, true);
                    foreach (var dependencyAssetPath in dependencies)
                    {
                        var dependencyAssetGuid = AssetDatabase.AssetPathToGUID(dependencyAssetPath);
                        if (assetHostRecordersMap.TryGetValue(dependencyAssetGuid, out var recorder))
                        {
                            recorder.hostAssetGuids.Add(sourceAsset.Guid);
                        }
                    }
                }
            }

            UnityEditor.EditorUtility.ClearProgressBar();

            foreach (var keyValuePair in assetHostRecordersMap)
            {
                var record = keyValuePair.Value;
                if (record.hostAssetGuids.Count == 0)
                {
                    continue;
                }

                record.Sort();
            }

            return assetHostRecordersMap.Values.ToList();
        }

        private static int GetUniqueIDBaseOnAssetReference(AssetHostRecorder recorder)
        {
            foreach (var keyValuePair in uniqueIdDic)
            {
                var haveSameHostAsset = true;
                var savedHostAssetList = keyValuePair.Key;
                if (savedHostAssetList.Count != recorder.hostAssetGuids.Count)
                {
                    haveSameHostAsset = false;
                }
                else
                {
                    for (var i = 0; i < savedHostAssetList.Count; i++)
                    {
                        if (savedHostAssetList[i] != recorder.hostAssetGuids[i])
                        {
                            haveSameHostAsset = false;
                            break;
                        }
                    }
                }

                if (haveSameHostAsset)
                {
                    return keyValuePair.Value;
                }
            }

            //没有同样的host
            var uniqueId = uniqueIdDic.Count;
            uniqueIdDic.Add(new List<string>(recorder.hostAssetGuids), uniqueId);
            return uniqueId;
        }

        private static void AssignAssetBundles(List<SourceAsset> sourceAssetList,
                                               List<AssetHostRecorder> assetHostRecorders)
        {
            var resourceCollection = new ResourceCollection();
            if (!resourceCollection.Load())
            {
                Debug.Log("Load asset bundle collection failure");
                return;
            }

            void addAssetToAssetBundle(string bundleName, SourceAsset sourceAsset)
            {
                if (!resourceCollection.HasResource(bundleName, null))
                {
                    if (!resourceCollection.AddResource(bundleName, null, null, LoadType.LoadFromFile, false))
                    {
                        Debug.LogError($"add asset bundle failed: {bundleName}");
                        return;
                    }

                    // Debug.Log($"add asset bundle: {bundleName}");
                }

                if (!resourceCollection.AssignAsset(sourceAsset.Guid, bundleName, null))
                {
                    Debug.LogError($"assign asset failed: asset:{sourceAsset.Path} asset bundle: {bundleName}");
                    return;
                }
            }

            resourceCollection.Clear();

            //处理美术之外资源,按照文件夹打包
            for (var i = 0; i < sourceAssetList.Count; i++)
            {
                var sourceAsset = sourceAssetList[i];
                if (sourceAsset.Path.StartsWith(GameResourceAssetPathRoot))
                {
                    //美术资源,不按照文件夹打包
                    continue;
                }

                UnityEditor.EditorUtility.DisplayProgressBar("搜集可载入资源",
                    sourceAsset.Path,
                    (float) i / sourceAssetList.Count);

                var assetBundleName = sourceAsset.Folder.FromRootPath;
                var directoryPath = $"{Application.dataPath}/Game/{sourceAsset.Folder.FromRootPath}";

                var findRootAssetBundle = false;
                foreach (var rootPath in assetBundleRootPaths)
                {
                    //该资源路径属于一个根ab包
                    if (assetBundleName.IndexOf(rootPath, StringComparison.Ordinal) == 0)
                    {
                        findRootAssetBundle = true;
                        assetBundleName = rootPath;
                        break;
                    }
                }

                // Debug.Log(directoryPath);
                if (!findRootAssetBundle)
                {
                    if (Directory.GetDirectories(directoryPath).Length > 0)
                    {
                        // Debug.Log($"{directoryPath} 有子目录");
                        assetBundleName = $"{assetBundleName}_base";
                    }
                }

                addAssetToAssetBundle(assetBundleName, sourceAsset);
            }

            UnityEditor.EditorUtility.ClearProgressBar();

            //处理美术资源,按照引用打包
            foreach (var recorder in assetHostRecorders)
            {
                if (recorder.hostAssetGuids.Count == 0)
                {
                    //没有被引用的资源 不进包
                    continue;
                }

                var uniqueId = GetUniqueIDBaseOnAssetReference(recorder);
                var assetBundleName = $"GameResources/{uniqueId}";
                addAssetToAssetBundle(assetBundleName, recorder.sourceAsset);
            }

            resourceCollection.Save();
        }
    }
}
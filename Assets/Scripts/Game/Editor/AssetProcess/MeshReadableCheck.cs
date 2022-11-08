using System.Collections.Generic;
using System.Linq;
using Game.Common;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor.AssetProcess
{
    /// <summary>
    /// 自动检查哪些mesh需要开启read/write
    /// </summary>
    public static class MeshReadableCheck
    {
        [MenuItem("Tools/性能优化/自动处理模型的ReadWrite选项")]
        public static void AutoSetMeshAssetReadWriteOption()
        {
            var meshReadableProcessSetting =
                AssetDatabase.LoadAssetAtPath<MeshReadableProcessSetting>(
                    "Assets/Build/MeshReadableProcessSetting.asset");

            var needEnableReadWriteModelPaths = new HashSet<string>();

            void handleMeshCollider(MeshCollider meshCollider)
            {
                var sharedMesh = meshCollider.sharedMesh;
                handleMesh(sharedMesh);
            }

            void handleMeshFilter(MeshFilter meshFilter)
            {
                var sharedMesh = meshFilter.sharedMesh;
                handleMesh(sharedMesh);
            }

            void handleMesh(Mesh mesh)
            {
                if (mesh != null)
                {
                    var modelPath = AssetDatabase.GetAssetPath(mesh);
                    if (!needEnableReadWriteModelPaths.Contains(modelPath))
                    {
                        needEnableReadWriteModelPaths.Add(modelPath);
                    }
                }
            }

            var assetGuids = AssetDatabase.FindAssets("t:prefab", meshReadableProcessSetting.RootAssetPaths);
            for (var i = 0; i < assetGuids.Length; i++)
            {
                var assetGuid = assetGuids[i];
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

                UnityEditor.EditorUtility.DisplayProgressBar("分析模型文件是否开启read/write...",
                    assetPath,
                    (float) i / assetGuids.Length);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                var meshColliders = prefab.GetComponentsInChildren<MeshCollider>();
                if (meshColliders.Length > 0)
                {
                    foreach (var meshCollider in meshColliders)
                    {
                        handleMeshCollider(meshCollider);
                    }
                }

                var flags = prefab.GetComponentsInChildren<MeshReadableFlag>();
                if (flags.Length > 0)
                {
                    foreach (var flag in flags)
                    {
                        if (flag.MeshFilters != null)
                        {
                            foreach (var meshFilter in flag.MeshFilters)
                            {
                                if (meshFilter == null)
                                {
                                    continue;
                                }

                                handleMeshFilter(meshFilter);
                            }
                        }
                    }
                }
            }

            //find all scene
            var activeScenePath = SceneManager.GetActiveScene().path;
            var sceneGuids = AssetDatabase.FindAssets("t:scene", new[] {"Assets/Game/Scene"});
            for (var i = 0; i < sceneGuids.Length; i++)
            {
                var guid = sceneGuids[i];
                var scenePath = AssetDatabase.GUIDToAssetPath(guid);
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                var labels = AssetDatabase.GetLabels(sceneAsset);
                if (labels.Contains("AssetBundleExclusive"))
                {
                    Debug.Log($"忽略场景{scenePath}");
                    continue;
                }

                var openScene = EditorSceneManager.OpenScene(scenePath);

                var meshColliders = Object.FindObjectsOfType<MeshCollider>(true);
                foreach (var meshCollider in meshColliders)
                {
                    handleMeshCollider(meshCollider);
                }
                
                var flags = Object.FindObjectsOfType<MeshReadableFlag>(true);
                foreach (var flag in flags)
                {
                    foreach (var meshFilter in flag.MeshFilters)
                    {
                        if (meshFilter == null)
                        {
                            continue;
                        }
                        
                        handleMeshFilter(meshFilter);
                    }
                }

                EditorSceneManager.CloseScene(openScene, false);
            }

            if (!string.IsNullOrEmpty(activeScenePath))
            {
                EditorSceneManager.OpenScene(activeScenePath);
            }

            //find all models
            var allModelGuids = AssetDatabase.FindAssets("t:model", new[] {"Assets/Game/GameResources"});
            for (var i = 0; i < allModelGuids.Length; i++)
            {
                var modelGuid = allModelGuids[i];
                var modelAssetPath = AssetDatabase.GUIDToAssetPath(modelGuid);
                UnityEditor.EditorUtility.DisplayProgressBar("启用模型的read/write...",
                    modelAssetPath,
                    (float) i / allModelGuids.Length);
                var modelImporter = ModelImporter.GetAtPath(modelAssetPath) as ModelImporter;
                var isEnableReadable = needEnableReadWriteModelPaths.Contains(modelAssetPath);
                if (modelImporter.isReadable != isEnableReadable)
                {
                    modelImporter.isReadable = isEnableReadable;
                    if (isEnableReadable)
                    {
                        Debug.Log($"打开模型{modelAssetPath}的read/write");
                    }

                    modelImporter.SaveAndReimport();
                }
            }

            AssetDatabase.Refresh();

            UnityEditor.EditorUtility.ClearProgressBar();
        }
    }
}
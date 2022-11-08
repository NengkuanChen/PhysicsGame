using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Editor.AssetProcess
{
    public class CommonAssetImporter : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets,
                                                   string[] deletedAssets,
                                                   string[] movedAssets,
                                                   string[] movedFromAssetPaths)
        {
            foreach (var importAssetPath in importedAssets)
            {
                TryAddTableFileAssetBundleLabel(importAssetPath);
            }
        }

        private void OnPostprocessModel(GameObject g)
        {
            var modelImporter = (ModelImporter) assetImporter;
            modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
        }

        private void OnPostprocessAudio(AudioClip arg)
        {
            if (assetPath.StartsWith("Assets/Game/Sound"))
            {
                var audioImporter = (AudioImporter) assetImporter;
                if (!audioImporter.forceToMono)
                {
                    audioImporter.forceToMono = true;
                    Debug.Log($"音频文件{assetPath} 强制设置为单声道");
                }
            }
        }

        private static void TryAddTableFileAssetBundleLabel(string assetPath)
        {
            if (!assetPath.StartsWith("Assets/Game"))
            {
                return;
            }

            if (assetPath.StartsWith("Assets/Game/Table/") || assetPath.EndsWith(".shadervariants"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                if (asset != null)
                {
                    AssetDatabase.SetLabels(asset, new[] {"AssetBundleInclusive"});
                }
            }

            if (assetPath.EndsWith(".hlsl"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                if (asset != null)
                {
                    AssetDatabase.SetLabels(asset, new[] {"AssetBundleExclusive"});
                }
            }
        }
    }
}
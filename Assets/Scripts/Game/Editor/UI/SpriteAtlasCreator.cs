using System;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace Game.Editor.UI
{
    public static class SpriteAtlasCreator
    {
        [MenuItem("Tools/自动将UI sprite归入对应的图集")]
        public static void AutoAssign()
        {
            var spriteRootPath = $"{Application.dataPath}/Game/UI/Sprites";
            var spriteDirectoryPaths = Directory.GetDirectories(spriteRootPath);
            foreach (var directoryPath in spriteDirectoryPaths)
            {
                var directoryInfo = new DirectoryInfo(directoryPath);
                var directoryname = directoryInfo.Name;
                var spriteAtlasFullPath = $"{directoryPath}/{directoryname}.spriteatlas";
                var spriteAtlasAssetPath =
                    spriteAtlasFullPath.Substring(spriteAtlasFullPath.IndexOf("Assets/", StringComparison.Ordinal));
                var spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(spriteAtlasAssetPath);
                if (spriteAtlas == null)
                {
                    spriteAtlas = new SpriteAtlas();
                    spriteAtlas.SetIncludeInBuild(true);

                    var setting = new SpriteAtlasPackingSettings
                    {
                        padding = 4,
                        enableRotation = true,
                        enableTightPacking = false
                    };

                    spriteAtlas.SetPackingSettings(setting);

                    AssetDatabase.CreateAsset(spriteAtlas, spriteAtlasAssetPath);
                    Debug.Log($"create sprite atlas {spriteAtlasAssetPath}");
                    AssetDatabase.Refresh();
                }

                AssetDatabase.SetLabels(spriteAtlas, new[] {"AssetBundleInclusive"});

                //clear sprite atlas
                var packables = spriteAtlas.GetPackables();
                spriteAtlas.Remove(packables);

                //find all sprites
                var allFileInfos = directoryInfo.GetFiles();
                foreach (var fileInfo in allFileInfos)
                {
                    if (fileInfo.Extension == ".meta")
                    {
                        continue;
                    }

                    if (fileInfo.Extension == ".spriteatlas")
                    {
                        continue;
                    }

                    var fileFullPath = EditorUtility.GetRegularPath(fileInfo.FullName);
                    var assetPath = EditorUtility.GetProjectPath(fileFullPath);
                    var loadedAsset = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (loadedAsset != null)
                    {
                        // Debug.Log(loadedAsset.name);
                        spriteAtlas.Add(new Object[] {loadedAsset});
                    }
                    else
                    {
                        Debug.LogError($"can not load sprite {assetPath}");
                    }
                }
            }
        }
    }
}
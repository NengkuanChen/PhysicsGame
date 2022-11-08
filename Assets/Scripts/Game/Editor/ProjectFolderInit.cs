using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class ProjectFolderInit
    {
        [MenuItem("Game Framework/初始化工程目录结构")]
        public static void InitProjectFolderTree()
        {
            var assetRootPath = Application.dataPath;
            var gameRootPath = $"{assetRootPath}/Game";
            if (!Directory.Exists(gameRootPath))
            {
                Directory.CreateDirectory(gameRootPath);
            }

            var subDirectoryNames = new[]
            {
                "GameResources", "Entity", "Scene", "Setting", "Shader", "Sound", "Table", "UI", "UI/Font",
                "UI/Form", "UI/Sprites"
            };
            foreach (var subDirectoryName in subDirectoryNames)
            {
                var subDirectoryPath = $"{gameRootPath}/{subDirectoryName}";
                if (!Directory.Exists(subDirectoryPath))
                {
                    Directory.CreateDirectory(subDirectoryPath);
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
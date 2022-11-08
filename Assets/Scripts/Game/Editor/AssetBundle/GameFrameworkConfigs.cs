using System.IO;
using UnityEngine;
using UnityGameFramework.Editor;
using UnityGameFramework.Editor.ResourceTools;

namespace Game.Editor.AssetBundle
{
    public static class GameFrameworkConfigs
    {
        [BuildSettingsConfigPath]
        public static string BuildSettingsConfig =
            GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Configs/BuildSettings.xml"));

        [ResourceBuilderConfigPath]
        public static string AssetBundleBuilderConfig =
            GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Configs/AssetBundleBuilder.xml"));

        [ResourceEditorConfigPath]
        public static string AssetBundleEditorConfig =
            GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Configs/AssetBundleEditor.xml"));

        [ResourceCollectionConfigPath]
        public static string AssetBundleCollectionConfig =
            GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Configs/AssetBundleCollection.xml"));
    }
}
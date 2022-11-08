using System.IO;
using UnityEditor;

namespace Game.Editor.AssetBundle
{
    public static class AssetExclusiveTool
    {
        [MenuItem("Assets/AssetBundleTool/资源不打入AssetBundle")]
        public static void ExclusiveFromAssetBundle()
        {
            var selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            var exclusiveLabel = new[] {"AssetBundleExclusive"};

            foreach (var asset in selectedAssets)
            {
                var assetPath = AssetDatabase.GetAssetPath(asset);
                if (Path.HasExtension(assetPath))
                {
                    AssetDatabase.SetLabels(asset, exclusiveLabel);
                    Log.Info($"Exclude asset {assetPath} from asset bundle build");
                }
            }
        }

        [MenuItem("Assets/AssetBundleTool/资源打入AssetBundle")]
        public static void IncludeToAssetBundle()
        {
            var selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            var inclusiveLabel = new[] {"AssetBundleInclusive"};

            foreach (var asset in selectedAssets)
            {
                var assetPath = AssetDatabase.GetAssetPath(asset);
                if (Path.HasExtension(assetPath))
                {
                    AssetDatabase.SetLabels(asset, inclusiveLabel);
                }
            }
        }
    }
}
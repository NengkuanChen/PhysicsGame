using Game.GameVariant;

namespace Game.Utility
{
    public static class AssetPathUtility
    {
        public static string GetSceneAssetPath(string relativePath)
        {
            var gameVariantSystem = GameVariantSystem.Get();
            return gameVariantSystem.GetVariantSceneAssetPath(relativePath);
        }

        public static string GetEntityAssetPath(string relativePath)
        {
            var gameVariantSystem = GameVariantSystem.Get();
            return gameVariantSystem.GetVariantEntityAssetPath(relativePath);
        }

        public static string GetUIFormAssetPath(string relativePath)
        {
            var gameVariantSystem = GameVariantSystem.Get();
            return gameVariantSystem.GetVariantUIFormAssetPath(relativePath);
        }

        public static string GetCommonAssetPath(string relativePath)
        {
            var gameVariantSystem = GameVariantSystem.Get();
            return gameVariantSystem.GetVariantCommonAssetPath(relativePath);
        }

        public static string GetSpritePath(string relativePath)
        {
            var gameVariantSystem = GameVariantSystem.Get();
            return gameVariantSystem.GetVariantUISpriteAssetPath(relativePath);
        }
    }
}
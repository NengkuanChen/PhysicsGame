using System;
using Cysharp.Threading.Tasks;
using Game.GameSystem;
using Game.Utility;

namespace Game.GameVariant
{
    /// <summary>
    /// 游戏内AB Test功能系统
    /// </summary>
    public class GameVariantSystem : SystemBase
    {
        public static GameVariantSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as GameVariantSystem;
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private GameVariantSetting.VariantConfig currentVariantConfig;
        private VariantAssetData currentVariantAssetData;

        public static GameVariantLogic CurrentLogic
        {
            get
            {
                var system = Get();
                if (system?.currentVariantConfig == null)
                {
                    return GameVariantLogic.Default;
                }

                return system.currentVariantConfig.Logic;
            }
        }

        public GameVariantSystem(GameVariantSetting.VariantConfig currentVariantConfig)
        {
            this.currentVariantConfig = currentVariantConfig;
        }

        public async UniTask LoadVariantAssetDataAsync()
        {
            if (currentVariantConfig == null)
            {
                return;
            }

            var assetDataAssetPath = $"Assets/Game/Variants/{currentVariantConfig.Folder}/VariantAssetData.asset";
            currentVariantAssetData = await ResourceUtility.LoadAssetAsync<VariantAssetData>(assetDataAssetPath, false);
            if (currentVariantAssetData == null)
            {
                throw new Exception($"无法载入{assetDataAssetPath}");
            }

            currentVariantAssetData.Init();
        }

        private string FindVariantPath(string relativePathFromGameFolder)
        {
            if (currentVariantConfig == null || !currentVariantAssetData.ContainAsset(relativePathFromGameFolder))
            {
                return $"Assets/Game/{relativePathFromGameFolder}";
            }

            return $"Assets/Game/Variants/{currentVariantConfig.Folder}/{relativePathFromGameFolder}";
        }

        public string GetVariantSceneAssetPath(string relativePath)
        {
            var relativePathFromGameFolder = $"Scene/{relativePath}.unity";
            return FindVariantPath(relativePathFromGameFolder);
        }

        public string GetVariantEntityAssetPath(string relativePath)
        {
            var relativePathFromGameFolder = $"Entity/{relativePath}.prefab";
            return FindVariantPath(relativePathFromGameFolder);
        }

        public string GetVariantUIFormAssetPath(string relativePath)
        {
            var relativePathFromGameFolder = $"UI/Form/{relativePath}.prefab";
            return FindVariantPath(relativePathFromGameFolder);
        }

        public string GetVariantUISpriteAssetPath(string relativePath)
        {
            var relativePathFromGameFolder = $"UI/Sprites/{relativePath}.png";
            return FindVariantPath(relativePathFromGameFolder);
        }

        public string GetVariantCommonAssetPath(string relativePath)
        {
            return FindVariantPath(relativePath);
        }
    }
}
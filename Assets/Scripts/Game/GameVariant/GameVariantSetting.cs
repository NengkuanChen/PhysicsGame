using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using Game.GameSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.GameVariant
{
    [CreateAssetMenu(fileName = "GameVariantSetting", menuName = "Settings/GameVariant/GameVariantSetting", order = 0)]
    public class GameVariantSetting : ScriptableObject
    {
        [Serializable]
        public class VariantConfig
        {
            [SerializeField, LabelText("配置名称")]
            private string name = "Variant";
            public string Name => name;
            [SerializeField, LabelText("资源目录"), FolderPath(ParentFolder = "Assets/Game/Variants")]
            private string folder;
            public string Folder => folder;
            [SerializeField, LabelText("代码逻辑类型")]
            private GameVariantLogic logic;
            public GameVariantLogic Logic => logic;
            [SerializeField, LabelText("生效权重"), Min(0)]
            private int weight = 100;
            public int Weight => weight;
        }

        [SerializeField, LabelText("资源版本"), Min(1), InfoBox("资源版本和当前版本不一致的话,强制重新选择资源")]
        private int variantVersion = 1;
        public int VariantVersion => variantVersion;
        [SerializeField, LabelText("默认资源权重"), Min(0)]
        private int defaultVariantWeight = 100;
        public int DefaultVariantWeight => defaultVariantWeight;

        [SerializeField, LabelText("多资源配置")]
        private VariantConfig[] variantConfigs;
        public VariantConfig[] VariantConfigs => variantConfigs;

        public bool CheckIsUseVariant(out VariantConfig variantConfig)
        {
            variantConfig = null;
            if (variantConfigs == null || variantConfigs.Length == 0)
            {
                return false;
            }

        #if UNITY_EDITOR
            if (enableVariantTest)
            {
                foreach (var config in variantConfigs)
                {
                    if (config.Name == testConfigName)
                    {
                        Log.Info($"测试模式,强制使用配置{config.Name}");
                        variantConfig = config;
                        return true;
                    }
                }
            }
        #endif

            var gameDataSystem = GameDataSystem.Get();
            //尝试使用已保存的资源变体选择
            if (gameDataSystem.VariantVersion == variantVersion)
            {
                var savedVariantName = gameDataSystem.VariantName;
                if (savedVariantName == "Default")
                {
                    //使用默认资源
                    Log.Info("使用默认配置");
                    return false;
                }

                foreach (var config in variantConfigs)
                {
                    if (config.Name == savedVariantName)
                    {
                        variantConfig = config;
                        Log.Info($"使用变体配置{config.Name}");
                        return true;
                    }
                }
            }
            else
            {
                Log.Info($"资源版本更换.from {gameDataSystem.VariantVersion} to {variantVersion},强制重新选择资源");
                gameDataSystem.VariantVersion = variantVersion;
            }

            Log.Info("开始选择变体配置");
            var totalWeight = defaultVariantWeight;
            foreach (var config in variantConfigs)
            {
                totalWeight += config.Weight;
            }

            var randomWeight = Random.Range(0, totalWeight + 1);
            if (randomWeight <= defaultVariantWeight)
            {
                Log.Info("使用默认资源");
                gameDataSystem.VariantName = "Default";
                return false;
            }

            randomWeight -= defaultVariantWeight;
            foreach (var config in variantConfigs)
            {
                if (randomWeight <= config.Weight)
                {
                    variantConfig = config;
                    Log.Info($"使用变体配置{config.Name}");
                    gameDataSystem.VariantName = config.Name;
                    return true;
                }

                randomWeight -= config.Weight;
            }

            gameDataSystem.VariantName = "Default";
            return false;
        }

    #if UNITY_EDITOR
        [BoxGroup("测试"), SerializeField, LabelText("开启资源测试")]
        private bool enableVariantTest;
        public bool EnableVariantTest => enableVariantTest;
        [BoxGroup("测试"), SerializeField, LabelText("测试配置"), ValueDropdown("GetAllConfigNames"),
         ShowIf("@enableVariantTest")]
        private string testConfigName;
        public string TestConfigName => testConfigName;

        private IEnumerable GetAllConfigNames()
        {
            var result = new List<ValueDropdownItem<string>>();
            if (variantConfigs != null)
            {
                foreach (var config in variantConfigs)
                {
                    result.Add(new ValueDropdownItem<string>(config.Name, config.Name));
                }
            }

            return result;
        }

        [Button(ButtonSizes.Large, Name = "开始生成变体资源数据")]
        private void Editor_CreateVariantAssetData()
        {
            if (variantConfigs == null || variantConfigs.Length == 0)
            {
                return;
            }

            for (var i = 0; i < variantConfigs.Length; i++)
            {
                var config = variantConfigs[i];
                EditorUtility.DisplayProgressBar("请稍候", $"变体资源{config.Folder}搜索中", (float) i / variantConfigs.Length);
                // var variantRootPath = $"Assets/Game/Variants/{config.Folder}";
                var variantAssetDataAssetPath = $"Assets/Game/Variants/{config.Folder}/VariantAssetData.asset";
                var assetData = AssetDatabase.LoadAssetAtPath<VariantAssetData>(variantAssetDataAssetPath);
                var isCreatedData = false;
                if (assetData == null)
                {
                    isCreatedData = true;
                    assetData = CreateInstance<VariantAssetData>();
                    Debug.Log($"create at {variantAssetDataAssetPath}");
                }

                assetData.rootFolderName = config.Folder;
                assetData.Editor_ScanVariantAssets();
                if (isCreatedData)
                {
                    AssetDatabase.CreateAsset(assetData, variantAssetDataAssetPath);
                }
            }

            EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();
        }

        [Button(ButtonSizes.Large, Name = "检查不同资源")]
        private void Editor_CheckDifferentAssets()
        {
            CheckDifferentVariantAssetWindow.ShowWindow(this);

        }
    #endif
    }
}
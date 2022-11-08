#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.GameVariant
{
    public class CheckDifferentVariantAssetWindow : OdinEditorWindow
    {
        private GameVariantSetting variantSetting;

        public static void ShowWindow(GameVariantSetting variantSetting)
        {
            var window = GetWindow<CheckDifferentVariantAssetWindow>();
            window.titleContent = new GUIContent("检查不一致变体资源");
            window.minSize = new Vector2(500, 400);
            window.variantSetting = variantSetting;
            window.Show();
        }

        [Serializable]
        private class DifferentAssetsInfo
        {
            [SerializeField, LabelText("属于配置名称"), ReadOnly]
            public string variantName;
            [SerializeField, LabelText("不同于默认配置的资源名称"), ListDrawerSettings(Expanded = true), ReadOnly]
            public List<string> differentAssetRelativePaths;
        }

        [SerializeField, LabelText("差异配置列表"), ListDrawerSettings(Expanded = true), ReadOnly]
        private List<DifferentAssetsInfo> differentAssetsInfos = new List<DifferentAssetsInfo>();

        [Button(ButtonSizes.Large, Name = "开始检查")]
        private void StartCheck()
        {
            differentAssetsInfos.Clear();
            if (variantSetting.VariantConfigs == null || variantSetting.VariantConfigs.Length == 0)
            {
                return;
            }

            for (var i = 0; i < variantSetting.VariantConfigs.Length; i++)
            {
                var config = variantSetting.VariantConfigs[i];
                var variantAssetDataAssetPath = $"Assets/Game/Variants/{config.Folder}/VariantAssetData.asset";
                var assetData = AssetDatabase.LoadAssetAtPath<VariantAssetData>(variantAssetDataAssetPath);
                if (assetData != null)
                {
                    var differentAssetPaths = assetData.Editor_DetectDifferentAssets();
                    differentAssetsInfos.Add(new DifferentAssetsInfo
                    {
                        variantName = config.Name,
                        differentAssetRelativePaths = differentAssetPaths
                    });
                }
            }
        }
    }
}
#endif
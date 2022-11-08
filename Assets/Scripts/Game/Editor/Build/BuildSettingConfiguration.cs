using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using DingtalkChatbotSdk;
using Game.Advertisement;
using LunarConsolePluginInternal;
using Sirenix.OdinInspector;
using StringExtensionLibrary;
using UnityEditor;
using UnityEngine;
#if OCF_REPORT_ENABLE
using Facebook.Unity.Editor;
using Facebook.Unity.Settings;

#endif

namespace Game.Editor.Build
{
    public abstract class BuildSettingConfiguration : ScriptableObject
    {
        [SerializeField, LabelText("构建AssetBundle")]
        private bool buildAssetBundle = true;
        public bool BuildAssetBundle => buildAssetBundle;
        [SerializeField, LabelText("开启日志"), OnValueChanged("OnEnableLogChanged")]
        private bool enableLog;
        public bool EnableLog => enableLog;
        [SerializeField, LabelText("开启LunarConsole"), OnValueChanged("OnEnableLunarConsoleChanged")]
        private bool enableLunarConsole;
        public bool EnableLunarConsole => enableLunarConsole;
        [SerializeField, LabelText("开启调试模式")]
        private bool enableDevelopmentMode;
        public bool EnableDevelopmentMode => enableDevelopmentMode;
        [SerializeField, LabelText("钉钉推送打包完成通知")]
        private bool enableDingTalkInform = true;
        public bool EnableDingTalkInform => enableDingTalkInform;

        private PackageInfoConfiguration packageInfoConfiguration;
        public PackageInfoConfiguration PackageInfoConfiguration
        {
            get => packageInfoConfiguration;
            set => packageInfoConfiguration = value;
        }

        public void SendDingTalkMessage(string msg)
        {
            if (!enableDingTalkInform)
            {
                return;
            }

            async UniTaskVoid sendMessageAsync()
            {
                var result =
                    await DingDingClient.SendMessageAsync(DingTalkConstant.HookURL, $"通知\n{msg}", isAtAll: false);
                Debug.Log($"发送完成 {result.ErrCode} {result.ErrMsg}");
            }

            sendMessageAsync().Forget();
        }

        private const string LogEnabledSymbol = "ENABLE_LOG";

        private void OnEnable()
        {
            enableLog = BuildSymbolHandler.IsSymbolEnabled(LogEnabledSymbol);

            enableLunarConsole = LunarConsoleConfig.consoleEnabled;
            enableDevelopmentMode = EditorUserBuildSettings.development;
        }

        private void OnEnableLogChanged()
        {
            if (enableLog)
            {
                BuildSymbolHandler.EnableSymbol(LogEnabledSymbol);
            }
            else
            {
                BuildSymbolHandler.DisableSymbol(LogEnabledSymbol);
            }
        }

        protected virtual void OnEnableLunarConsoleChanged()
        {
            if (enableLunarConsole)
            {
                LunarConsoleEditorInternal.Installer.EnablePlugin();
            }
            else
            {
                LunarConsoleEditorInternal.Installer.DisablePlugin();
            }
        }

        protected virtual void ApplyPackageInfo()
        {
            PlayerSettings.productName = packageInfoConfiguration.AppName;
            PlayerSettings.bundleVersion = packageInfoConfiguration.Version;
            PlayerSettings.companyName = packageInfoConfiguration.CompanyName;
            //设置图标
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] {packageInfoConfiguration.Icon});

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

        #if OCF_REPORT_ENABLE
            FacebookSettings.AppLabels.Clear();
            FacebookSettings.AppLabels.Add(packageInfoConfiguration.AppName);
            FacebookSettings.AppIds.Clear();
            FacebookSettings.AppIds.Add(packageInfoConfiguration.FacebookId);
            ManifestMod.GenerateManifest();
            UnityEditor.EditorUtility.SetDirty(FacebookSettings.Instance);
        #endif

            AssetDatabase.SaveAssets();
        }

        [Button(ButtonSizes.Gigantic, Name = "开始构建"), ShowIf("@!EditorApplication.isCompiling"),
         PropertySpace(SpaceBefore = 30)]
        protected virtual void StartBuild()
        {
            ApplyPackageInfo();
            if (enableDevelopmentMode)
            {
                EditorUserBuildSettings.development = enableDevelopmentMode;
            }
        }

        protected AdvertisementSetting[] GetAllAdvertisementSettings()
        {
            var guids = AssetDatabase.FindAssets("t:advertisementsetting",
                new[] {"Assets/Game/Setting", "Assets/Game/Variants"});
            var result = new List<AdvertisementSetting>();
            foreach (var guid in guids)
            {
                result.Add(AssetDatabase.LoadAssetAtPath<AdvertisementSetting>(AssetDatabase.GUIDToAssetPath(guid)));
            }

            return result.ToArray();
        }
    }
}
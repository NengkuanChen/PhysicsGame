using Game.Advertisement;
using Game.Report;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor.Build
{
    [CreateAssetMenu(fileName = "BuildSettingConfiguration(IOS)",
        menuName = "Build/BuildSettingConfiguration(IOS)",
        order = 0)]
    public class IOSBuildSettingConfiguration : BuildSettingConfiguration
    {
        [SerializeField, LabelText("清空构建(Replace Mode)")]
        private bool replaceBuild;
        public bool ReplaceBuild => replaceBuild;

        private IOSPackageInfoConfiguration IOSPackageInfoConfiguration =>
            PackageInfoConfiguration as IOSPackageInfoConfiguration;

        protected override void ApplyPackageInfo()
        {
            base.ApplyPackageInfo();

            var reportSetting = AssetDatabase.LoadAssetAtPath<ReportSetting>("Assets/Game/Setting/ReportSetting.asset");
            var advertisementSettings = GetAllAdvertisementSettings();

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, IOSPackageInfoConfiguration.PackageName);
            PlayerSettings.iOS.buildNumber = IOSPackageInfoConfiguration.BundleVersionCode.ToString();
            if (reportSetting != null)
            {
                reportSetting.SetIosKey(IOSPackageInfoConfiguration.AppsflyerDevKey);
                reportSetting.SetIosAppId(IOSPackageInfoConfiguration.IOSAppId.ToString());
                UnityEditor.EditorUtility.SetDirty(reportSetting);
            }

            /*
            if (advertisementSettings != null && advertisementSettings.Length > 0)
            {
                foreach (var advertisementSetting in advertisementSettings)
                {
                    advertisementSetting.Key = IOSPackageInfoConfiguration.MAXKey;
                    advertisementSetting.IOSBannerKey = IOSPackageInfoConfiguration.BannerAdKey;
                    advertisementSetting.IOSInterstitialKey = IOSPackageInfoConfiguration.InterstitialAdKey;
                    advertisementSetting.IOSRewardKey = IOSPackageInfoConfiguration.RewardAdKey;
                    UnityEditor.EditorUtility.SetDirty(advertisementSetting);
                }
            }*/

        #if OCF_ADVERTISEMENT_ENABLE
            var appLovinSettings = AppLovinSettings.Instance;
            appLovinSettings.AdMobIosAppId = IOSPackageInfoConfiguration.AdmobAppId;
            UnityEditor.EditorUtility.SetDirty(appLovinSettings);
        #endif
            AssetDatabase.SaveAssets();
        }

        protected override void StartBuild()
        {
            base.StartBuild();

            if (BuildAssetBundle)
            {
                BuildTools.BuildIOSAssetBundles();
            }

            PlayerPrefs.SetInt(DingTalkConstant.EnableDingTalkSaveKey, EnableDingTalkInform ? 1 : 0);

            var result = BuildTools.BuildIOS_XCode_Project(replaceBuild, out var buildPath);
            if (result == BuildResult.Succeeded)
            {
                SendDingTalkMessage(
                    $"项目{PackageInfoConfiguration.AppName},版本{IOSPackageInfoConfiguration.Version}_{IOSPackageInfoConfiguration.BundleVersionCode},XCode工程导出成功");
                System.Diagnostics.Process.Start(buildPath);
            }
            else if (result == BuildResult.Failed || result == BuildResult.Unknown)
            {
                SendDingTalkMessage(
                    $"项目{PackageInfoConfiguration.AppName},版本{IOSPackageInfoConfiguration.Version}_{IOSPackageInfoConfiguration.BundleVersionCode},XCode工程导出失败");
            }
        }

        protected override void OnEnableLunarConsoleChanged()
        {
            base.OnEnableLunarConsoleChanged();
            //lunar console任何改变需要重新出包
            UnityEditor.EditorUtility.DisplayDialog("提示", "修改了LunarConsole模式，需要使用Replace模式", "ok");
            replaceBuild = true;
        }
    }
}
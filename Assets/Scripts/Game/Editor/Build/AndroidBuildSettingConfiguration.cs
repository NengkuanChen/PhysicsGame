using System;
using System.IO;
using Game.Advertisement;
using Game.Report;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor.Build
{
    [CreateAssetMenu(fileName = "BuildSettingConfiguration(Android)",
        menuName = "Build/BuildSettingConfiguration(Android)",
        order = 0)]
    public class AndroidBuildSettingConfiguration : BuildSettingConfiguration
    {
        [SerializeField, LabelText("构建AAB包")]
        private bool exportAAB;
        public bool ExportAab => exportAAB;
        [SerializeField, LabelText("构建为Android工程")]
        private bool buildAsAndroidProject;
        public bool BuildAsAndroidProject => buildAsAndroidProject;

        private AndroidPackageInfoConfiguration AndroidPackageInfoConfiguration =>
            PackageInfoConfiguration as AndroidPackageInfoConfiguration;

        protected override void ApplyPackageInfo()
        {
            base.ApplyPackageInfo();

            PlayerSettings.Android.renderOutsideSafeArea = true;

            var reportSetting = AssetDatabase.LoadAssetAtPath<ReportSetting>("Assets/Game/Setting/ReportSetting.asset");
            var advertisementSettings = GetAllAdvertisementSettings();

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,
                AndroidPackageInfoConfiguration.PackageName);
            PlayerSettings.keystorePass = "123456";
            PlayerSettings.keyaliasPass = "123456";
            PlayerSettings.Android.bundleVersionCode = AndroidPackageInfoConfiguration.BundleVersionCode;

            if (reportSetting != null)
            {
                reportSetting.SetAndroidKey(AndroidPackageInfoConfiguration.AppsflyerDevKey);
                UnityEditor.EditorUtility.SetDirty(reportSetting);
            }

            if (advertisementSettings != null && advertisementSettings.Length > 0)
            {
                foreach (var advertisementSetting in advertisementSettings)
                {
                    advertisementSetting.Key = AndroidPackageInfoConfiguration.MAXKey;
                    advertisementSetting.AndroidBannerKey = AndroidPackageInfoConfiguration.BannerAdKey;
                    advertisementSetting.AndroidInterstitialKey = AndroidPackageInfoConfiguration.InterstitialAdKey;
                    advertisementSetting.AndroidRewardKey = AndroidPackageInfoConfiguration.RewardAdKey;
                    UnityEditor.EditorUtility.SetDirty(advertisementSetting);
                }
            }

        #if OCF_ADVERTISEMENT_ENABLE
            var appLovinSettings = AppLovinSettings.Instance;
            appLovinSettings.AdMobAndroidAppId = AndroidPackageInfoConfiguration.AdmobAppId;
            UnityEditor.EditorUtility.SetDirty(appLovinSettings);
        #endif
            AssetDatabase.SaveAssets();
        }

        protected override void StartBuild()
        {
            base.StartBuild();

            EditorUserBuildSettings.buildAppBundle = exportAAB;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = buildAsAndroidProject;

            if (BuildAssetBundle)
            {
                BuildTools.BuildAndroidAssetBundles();
            }

            var result = BuildTools.BuildAndroid(AndroidPackageInfoConfiguration.AppName,
                AndroidPackageInfoConfiguration.Version,
                AndroidPackageInfoConfiguration.BundleVersionCode,
                out var outputFilePath);
            if (result == BuildResult.Succeeded)
            {
                SendDingTalkMessage(
                    $"项目{PackageInfoConfiguration.AppName},版本{AndroidPackageInfoConfiguration.Version}_{AndroidPackageInfoConfiguration.BundleVersionCode}," +
                    $"Android版本构建完成。来自{Environment.UserName}的电脑");
                var outputFolderPath = Path.GetDirectoryName(outputFilePath);
                System.Diagnostics.Process.Start(outputFolderPath);
            }
            else if (result == BuildResult.Failed || result == BuildResult.Unknown)
            {
                SendDingTalkMessage(
                    $"项目{PackageInfoConfiguration.AppName},版本{AndroidPackageInfoConfiguration.Version}_{AndroidPackageInfoConfiguration.BundleVersionCode},Android版本构建失败。来自{Environment.UserName}的电脑");
            }
        }
    }
}
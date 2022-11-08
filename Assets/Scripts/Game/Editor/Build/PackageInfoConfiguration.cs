using System;
using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Editor.Build
{
    public class PackageInfoConfiguration : ScriptableObject
    {
        [BoxGroup("基础信息"), SerializeField, LabelText("APP名称")]
        private string appName = "Game";
        public string AppName => appName;
        [BoxGroup("基础信息"), SerializeField, LabelText("公司名称")]
        private string companyName = "mpmf";
        public string CompanyName => companyName;
        [BoxGroup("基础信息"), SerializeField, Required, LabelText("图标"), PreviewField(100, ObjectFieldAlignment.Right)]
        private Texture2D icon;
        public Texture2D Icon => icon;
        [BoxGroup("基础信息"), SerializeField, LabelText("包名")]
        private string packageName = "com.mpmf.game";
        public string PackageName => packageName;
        [BoxGroup("基础信息"), SerializeField, LabelText("大版本号")]
        private string version = "1.0.0";
        public string Version => version;
        [BoxGroup("基础信息"), SerializeField, LabelText("小版本号"), LabelWidth(200), HorizontalGroup("基础信息/bundle_version")]
        private int bundleVersionCode = 1000;
        public int BundleVersionCode => bundleVersionCode;

        // [BoxGroup("基础信息"), Button(ButtonSizes.Small, Name = "设置为svn号"), HorizontalGroup("基础信息/bundle_version")]
        // private void SetBundleVersionAsSvnNumber()
        // {
        //     bundleVersionCode = BuildTools.GetLatestSVNRevisionNumber();
        // }

        [BoxGroup("基础信息"), Button(ButtonSizes.Small, Name = "根据当前时间设置"), HorizontalGroup("基础信息/bundle_version")]
        private void SetBundleVersionAsLastGitCommitDate()
        {
            var timeSpan = DateTime.Now - new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            bundleVersionCode = (int) timeSpan.TotalMinutes;
        }

        [BoxGroup("基础信息"), Button(ButtonSizes.Small, Name = "+1"), HorizontalGroup("基础信息/bundle_version")]
        private void IncreaseBundleVersion()
        {
            bundleVersionCode++;
        }

        [BoxGroup("打点"), SerializeField, LabelText("Facebook ID"), LabelWidth(200)]
        private string facebookId;
        public string FacebookId => facebookId;
        [BoxGroup("打点"), SerializeField, LabelText("AppsflyerID"), LabelWidth(200)]
        private string appsflyerDevKey;
        public string AppsflyerDevKey => appsflyerDevKey;

        [BoxGroup("广告"), SerializeField, LabelText("Max Key"), TextArea(2, 6)]
        private string maxKey;
        public string MAXKey => maxKey;
        [BoxGroup("广告"), SerializeField, LabelText("插页KEY")]
        private string interstitialAdKey;
        public string InterstitialAdKey => interstitialAdKey;
        [BoxGroup("广告"), SerializeField, LabelText("激励KEY")]
        private string rewardAdKey;
        public string RewardAdKey => rewardAdKey;
        [BoxGroup("广告"), SerializeField, LabelText("Banner KEY")]
        private string bannerAdKey;
        public string BannerAdKey => bannerAdKey;
        [BoxGroup("广告"), SerializeField, LabelText("AdMob App ID"), LabelWidth(200)]
        private string admobAppId;
        public string AdmobAppId => admobAppId;
    }
}
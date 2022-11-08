using Game.GameSystem;
using Game.Utility;
using UnityEngine;
#if OCF_REPORT_ENABLE
using AppsFlyerSDK;
using Facebook.Unity;
#endif
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace Game.Report
{
    /// <summary>
    /// 激励广告位置
    /// </summary>
    public static class RewardAdLocate
    {
    }

    public static class ReportParameterName
    {
        public const string GameLaunchTimes = "TotalLoginTimes";
        public const string TotalGameSeconds = "TotalGameLength";
        public const string DeviceId = "DeviceID";
        public const string Version = "Version";

        public const string ComputeShaderSupport = "ComputeShaderSupport";
        public const string GameVariantName = "GameVariantName";

        public const string RewardAdLocate = "RwType";

        public const string LoadTime = "LoadTime";
        public const string Level = "Level";
    }

    public static class ReportEventName
    {
        /// <summary>
        /// 第一次登陆
        /// </summary>
        public const string Register = "mpmf_Progress_Res";

        /// <summary>
        /// 开机loading
        /// </summary>
        public const string StartLoadingTime = "mpmf_Progress_StartLoading";
        /// <summary>
        /// 载入比赛时间
        /// </summary>
        public const string LevelLoadingTime = "mpmf_Progress_LevelLoading";

        /// <summary>
        /// 播放插页广告
        /// </summary>
        public const string PlayInterstitialAd = "mpmf_ad_Inter";

        /// <summary>
        /// 插页广告关闭
        /// </summary>
        public const string CloseInterstitialAd = "mpmf_Ad_WebAdClose";

        /// <summary>
        /// 关闭激励广告
        /// </summary>
        public const string CloseRewardAd = "mpmf_ad_reward";
        
        public const string ATTNotDetermined = "IOS_ATT_NoShow";
        public const string ATTAllowed = "IOS_ATT_Allow";
        public const string ATTDenied = "IOS_ATT_Refuse";
    }

    public class ReportEventSystem : SystemBase
    {
        public static ReportEventSystem Get()
        {
            return SystemEntry.GetSystem(id) as ReportEventSystem;
        }

        private static int id = UniqueIdGenerator.GetUniqueId();
        internal override int ID => id;

        private EventWriter eventWriter = new EventWriter();

        internal override void OnEnable()
        {
            base.OnEnable();
        #if OCF_REPORT_ENABLE
            var reportSetting = SettingUtility.ReportSetting;
            var key = reportSetting.AppsFlyerDevKeyAndroid;
            var appId = Application.identifier;
        #if UNITY_IOS
            key = reportSetting.AppsFlyerDevKeyIOS;
            appId = reportSetting.AppsFlyerIosAppId;
        #endif
            Log.Info($"appsflyer初始化.key {key}, appid {appId}");
            AppsFlyer.initSDK(key, appId);
            AppsFlyer.startSDK();

            //facebook
            FB.Init();
        #endif
        }

        /// <summary>
        /// 游戏启动报告
        /// </summary>
        public void ReportGameLaunch()
        {
            var gameDataSystem = GameDataSystem.Get();
            //首次启动游戏事件
            if (gameDataSystem.GameLaunchTimes == 1)
            {
                eventWriter.StartWrite(ReportEventName.Register);
                eventWriter.AddParameter(ReportParameterName.ComputeShaderSupport,
                    SystemInfo.supportsComputeShaders ? "1" : "0");
                eventWriter.FlushEvent();
            }
        }

        public void ReportLoginLoadingTime(long loadMilliSeconds)
        {
            eventWriter.StartWrite(ReportEventName.StartLoadingTime);
            eventWriter.AddParameter(ReportParameterName.LoadTime, loadMilliSeconds.ToString());
            eventWriter.FlushEvent();
        }

        public void ReportLevelLoadingTime(long loadMilliSeconds, int level)
        {
            eventWriter.StartWrite(ReportEventName.LevelLoadingTime);
            eventWriter.AddParameter(ReportParameterName.LoadTime, loadMilliSeconds.ToString());
            eventWriter.AddParameter(ReportParameterName.Level, level.ToString());
            eventWriter.FlushEvent();
        }
        
    #if OCF_ADVERTISEMENT_ENABLE && UNITY_IOS
        public void ReportATTStatus(ATTrackingStatusBinding.AuthorizationTrackingStatus status)
        {
            var eventName = ReportEventName.ATTNotDetermined;
            switch (status)
            {
                case ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED:
                    break;
                case ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED:
                    eventName = ReportEventName.ATTDenied;
                    break;
                case ATTrackingStatusBinding.AuthorizationTrackingStatus.DENIED:
                    eventName = ReportEventName.ATTDenied;
                    break;
                case ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED:
                    eventName = ReportEventName.ATTAllowed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }

            eventWriter.StartWrite(eventName);
            eventWriter.FlushEvent();
        }
    #endif
    }
}

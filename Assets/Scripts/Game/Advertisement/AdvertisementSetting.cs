using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Advertisement
{
    [CreateAssetMenu(fileName = "AdvertisementSetting", menuName = "Settings/AdvertisementSetting")]
    public class AdvertisementSetting : ScriptableObject
    {
        [SerializeField, Multiline]
        private string key;
        public string Key
        {
            get => key;
        #if UNITY_EDITOR
            set => key = value;
        #endif
        }

        [SerializeField, LabelText("android 插页key")]
        private string androidInterstitialKey;
    #if UNITY_EDITOR
        public string AndroidInterstitialKey
        {
            get => androidInterstitialKey;
            set => androidInterstitialKey = value;
        }
    #endif

        [SerializeField, LabelText("android 激励key")]
        private string androidRewardKey;
    #if UNITY_EDITOR
        public string AndroidRewardKey
        {
            get => androidRewardKey;
            set => androidRewardKey = value;
        }
    #endif

        [SerializeField, LabelText("android banner key")]
        private string androidBannerKey;
    #if UNITY_EDITOR
        public string AndroidBannerKey
        {
            get => androidBannerKey;
            set => androidBannerKey = value;
        }
    #endif

        [SerializeField, LabelText("ios 插页key")]
        private string iosInterstitialKey;
    #if UNITY_EDITOR
        public string IOSInterstitialKey
        {
            get => iosInterstitialKey;
            set => iosInterstitialKey = value;
        }
    #endif
        [SerializeField, LabelText("ios 激励key")]
        private string iosRewardKey;
    #if UNITY_EDITOR
        public string IOSRewardKey
        {
            get => iosRewardKey;
            set => iosRewardKey = value;
        }
    #endif

        [SerializeField, LabelText("ios banner key")]
        private string iosBannerKey;
    #if UNITY_EDITOR
        public string IOSBannerKey
        {
            get => iosBannerKey;
            set => iosBannerKey = value;
        }
    #endif


        [SerializeField, LabelText("第几关能够播放插页"), Min(1)]
        private int interstitialAdCanPlayMinLevel = 3;
        public int InterstitialAdCanPlayMinLevel => interstitialAdCanPlayMinLevel;
        [SerializeField, LabelText("插页CD")]
        private float interstitialAdCoolDownSeconds = 60;
        public float InterstitialAdCoolDownSeconds => interstitialAdCoolDownSeconds;
        [SerializeField, LabelText("激励广告播放成功是否重置插页CD")]
        private bool successRewardAdResetInterstitialAdCoolDown = true;
        public bool SuccessRewardAdResetInterstitialAdCoolDown => successRewardAdResetInterstitialAdCoolDown;


        public string InterstitialKey
        {
            get
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return iosInterstitialKey;
                }

                return androidInterstitialKey;
            }
        }
        public string RewardKey
        {
            get
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return iosRewardKey;
                }

                return androidRewardKey;
            }
        }
        public string BannerKey
        {
            get
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return iosBannerKey;
                }

                return androidBannerKey;
            }
        }
    }
}
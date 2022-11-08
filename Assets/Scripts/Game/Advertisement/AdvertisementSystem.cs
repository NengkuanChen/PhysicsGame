using System;
using Cysharp.Threading.Tasks;
using Game.GameSystem;
using Game.UI.Form;
using Game.Utility;
using UnityEngine;

namespace Game.Advertisement
{
    public class AdvertisementSystem : SystemBase
    {
        public static AdvertisementSystem Get()
        {
            return SystemEntry.GetSystem(id) as AdvertisementSystem;
        }

        private static int id = UniqueIdGenerator.GetUniqueId();
        internal override int ID => id;

        private AdLoader rewardAdLoader;
        private AdLoader interstitialAdLoader;
        private AdLoader bannerAdLoader;

        private Action onInterstitialAdPlayComplete;
        private Action<bool> onRewardAdPlayComplete;

        private float lastInterstitialShownTime;

        /// <summary>
        /// 插页广告播放次数
        /// </summary>
        private int interstitialAdPlayedCount;
        public int InterstitialAdPlayedCount => interstitialAdPlayedCount;

        internal override void OnEnable()
        {
            base.OnEnable();
            var advertisementSetting = SettingUtility.AdvertisementSetting;

        #if OCF_ADVERTISEMENT_ENABLE
             rewardAdLoader = new AdLoader((retryTimes) =>
            {
                Log.LogAdvertisement($"start load reward ad,retry times: {retryTimes}");
                MaxSdk.LoadRewardedAd(advertisementSetting.RewardKey);
            });
            interstitialAdLoader = new AdLoader((retryTimes) =>
            {
                Log.LogAdvertisement($"start load interstitial ad, retry time: {retryTimes}");
                MaxSdk.LoadInterstitial(advertisementSetting.InterstitialKey);
            });
            bannerAdLoader = new AdLoader((retryTimes) =>
            {
                Log.LogAdvertisement($"start load banner ad, retry times: {retryTimes}");
                MaxSdk.CreateBanner(advertisementSetting.BannerKey, MaxSdkBase.BannerPosition.BottomCenter);
            });

            MaxSdkCallbacks.OnSdkInitializedEvent += configuration =>
            {
                Log.LogAdvertisement("max initialize complete");

                rewardAdLoader.TryLoad();
                interstitialAdLoader.TryLoad();
                bannerAdLoader.TryLoad();
            };

            //max
            MaxSdk.SetSdkKey(advertisementSetting.Key);
            MaxSdk.InitializeSdk();

            MaxSdkCallbacks.OnBannerAdLoadedEvent += OnBannerAdLoaded;
            MaxSdkCallbacks.OnBannerAdLoadFailedEvent += OnBannerAdLoadFailed;

            MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoaded;
            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialLoadFailed;
            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += OnInterstitialAdFailedToDisplay;
            MaxSdkCallbacks.OnInterstitialDisplayedEvent += OnInterstitialAdDisplayed;
            MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialHidden;

            MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoaded;
            MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdLoadFailed;
            MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplay;
            MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayed;
            MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClicked;
            MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardAdHidden;
            MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedReward;
        #endif
        }

    #if OCF_ADVERTISEMENT_ENABLE
        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            var unscaledDeltaTime = Time.unscaledDeltaTime;

            bannerAdLoader.Update(unscaledDeltaTime);
            interstitialAdLoader.Update(unscaledDeltaTime);
            rewardAdLoader.Update(unscaledDeltaTime);
        }

        private void OnBannerAdLoaded(string obj)
        {
            Log.LogAdvertisement($"banner load success, {obj}");
            bannerAdLoader.OnLoadSuccess();
            MaxSdk.ShowBanner(SettingUtility.AdvertisementSetting.BannerKey);
        }

        private void OnBannerAdLoadFailed(string arg1, int arg2)
        {
            Log.LogAdvertisement($"banner load failed, {arg1}, {arg2}");
            bannerAdLoader.OnLoadFailure();
            bannerAdLoader.TryLoad();
        }

        private void OnInterstitialLoaded(string obj)
        {
            Log.LogAdvertisement("interstitial ad load complete");
            interstitialAdLoader.OnLoadSuccess();
        }

        private void OnInterstitialLoadFailed(string arg1, int arg2)
        {
            Log.LogAdvertisement($"interstitial load failed. arg1: {arg1}, arg2: {arg2}");
            interstitialAdLoader.OnLoadFailure();
            interstitialAdLoader.TryLoad();
        }

        private void OnInterstitialAdFailedToDisplay(string arg1, int arg2)
        {
            Log.LogAdvertisement($"interstitial display failed. arg1: {arg1}, arg2: {arg2}");

            interstitialAdLoader.OnLoadFailure();
            interstitialAdLoader.TryLoad();

            EnableBlock(false);
            onInterstitialAdPlayComplete?.Invoke();
            onInterstitialAdPlayComplete = null;
        }

        private void OnInterstitialAdDisplayed(string obj)
        {
            Log.LogAdvertisement("interstitial ad displayed");
        }

        private void OnInterstitialHidden(string obj)
        {
            Log.LogAdvertisement("interstitial ad hidden");

            interstitialAdLoader.TryLoad();

            EnableBlock(false);

            lastInterstitialShownTime = Time.unscaledTime;
            interstitialAdPlayedCount++;

            onInterstitialAdPlayComplete?.Invoke();
            onInterstitialAdPlayComplete = null;

            OnStopPlayAdvertisement();
        }

        private void OnRewardedAdLoaded(string obj)
        {
            Log.LogAdvertisement("reward load complete");
            rewardAdLoader.OnLoadSuccess();
        }

        private void OnRewardedAdLoadFailed(string arg1, int arg2)
        {
            Log.LogAdvertisement($"reward load failed. arg1: {arg1}, arg2: {arg2}");
            rewardAdLoader.OnLoadFailure();
            rewardAdLoader.TryLoad();
        }

        private void OnRewardedAdFailedToDisplay(string arg1, int arg2)
        {
            Log.LogAdvertisement($"reward failed to display. arg1: {arg1}, arg2: {arg2}");
            rewardAdLoader.OnLoadFailure();
            rewardAdLoader.TryLoad();

            EnableBlock(false);

            onRewardAdPlayComplete?.Invoke(false);
            onRewardAdPlayComplete = null;

            OnStopPlayAdvertisement();
        }

        private void OnRewardedAdDisplayed(string obj)
        {
            Log.LogAdvertisement("reward displayed");
            rewardAdLoader.TryLoad();
        }

        private void OnRewardedAdClicked(string obj)
        {
            Log.LogAdvertisement("reward ad clicked");
            rewardAdLoader.TryLoad();
        }

        private void OnRewardAdHidden(string obj)
        {
            Log.LogAdvertisement("reward ad dismissed");
            rewardAdLoader.TryLoad();

            EnableBlock(false);

            onRewardAdPlayComplete?.Invoke(false);
            onRewardAdPlayComplete = null;

            OnStopPlayAdvertisement();
        }

        private void OnRewardedAdReceivedReward(string arg1, MaxSdkBase.Reward arg2)
        {
            Log.LogAdvertisement("reward ad receive reward");
            rewardAdLoader.TryLoad();

            EnableBlock(false);

            onRewardAdPlayComplete?.Invoke(true);
            onRewardAdPlayComplete = null;

            if (SettingUtility.AdvertisementSetting.SuccessRewardAdResetInterstitialAdCoolDown)
            {
                lastInterstitialShownTime = Time.unscaledTime;
            }

            OnStopPlayAdvertisement();
        }
    #endif

        public bool CanShowInterstitial()
        {
            if (Time.unscaledTime - lastInterstitialShownTime <
                SettingUtility.AdvertisementSetting.InterstitialAdCoolDownSeconds)
            {
                Log.LogAdvertisement("can not show interstitial ad because cool down");
                return false;
            }

        #if OCF_ADVERTISEMENT_ENABLE
            var result = MaxSdk.IsInterstitialReady(SettingUtility.AdvertisementSetting.InterstitialKey);
            if (!result)
            {
                Log.LogAdvertisement("can not show interstitial ad because not loaded");
            }

            return result;
        #else
            return false;
        #endif
        }

        public async UniTask ShowInterstitialAsync()
        {
            var isComplete = false;
            ShowInterstitial(() => isComplete = true);
            while (!isComplete)
            {
                await UniTask.Yield();
            }
        }

        public void ShowInterstitial(Action onComplete = null)
        {
        #if OCF_ADVERTISEMENT_ENABLE
            if (CanShowInterstitial())
            {
                Log.LogAdvertisement("show interstitial ad");

                OnStartPlayAdvertisement();

                onInterstitialAdPlayComplete = onComplete;
                EnableBlock(true);

                MaxSdk.ShowInterstitial(SettingUtility.AdvertisementSetting.InterstitialKey);
                return;
            }
        #endif

            Log.LogAdvertisement("can not show interstitial ad now");
            onInterstitialAdPlayComplete = null;
            EnableBlock(false);
        }

        public bool CanShowReward()
        {
        #if OCF_ADVERTISEMENT_ENABLE
            return MaxSdk.IsRewardedAdReady(SettingUtility.AdvertisementSetting.RewardKey);
        #else
            return false;
        #endif
        }

        public async UniTask<bool> ShowRewardAsync()
        {
            var result = false;
            var isComplete = false;
            ShowReward(success =>
            {
                isComplete = true;
                result = success;
            });
            while (!isComplete)
            {
                await UniTask.Yield();
            }

            return result;
        }

        public void ShowReward(Action<bool> onComplete)
        {
            Log.LogAdvertisement("show reward ad");

        #if OCF_ADVERTISEMENT_ENABLE
            if (CanShowReward())
            {
                OnStartPlayAdvertisement();

                onRewardAdPlayComplete = onComplete;
                EnableBlock(true);

                MaxSdk.ShowRewardedAd(SettingUtility.AdvertisementSetting.RewardKey);
                return;
            }
        #endif

            onRewardAdPlayComplete?.Invoke(false);
            onRewardAdPlayComplete = null;
            EnableBlock(false);
        }

        private void EnableBlock(bool isEnable)
        {
            if (UIUtility.GetForm(BlackForm.UniqueId) is BlackForm blackForm)
            {
                blackForm.EnableBlock(isEnable);
            }
        }

        private void OnStartPlayAdvertisement()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                AudioListener.volume = 0f;

                var gameMainCamera = GameMainCamera.Current;
                if (gameMainCamera != null)
                {
                    gameMainCamera.EnableRendering(false);
                }

                if (UICamera.Current != null)
                {
                    UICamera.Current.EnableRendering(false);
                }
            }
        }

        private void OnStopPlayAdvertisement()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                var gameDataSystem = GameDataSystem.Get();
                if (!gameDataSystem.IsMute)
                {
                    AudioListener.volume = 1f;
                }

                if (UICamera.Current != null)
                {
                    UICamera.Current.EnableRendering(true);
                }

                var gameMainCamera = GameMainCamera.Current;
                if (gameMainCamera != null)
                {
                    gameMainCamera.EnableRendering(true);
                }
            }
        }
    }
}
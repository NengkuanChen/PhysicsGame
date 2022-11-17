using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
#if OCF_REPORT_ENABLE
using Facebook.Unity;
using AppsFlyerSDK;
#endif
using Game.Advertisement;
using Game.Common;
using Game.Entity;
using Game.GameSystem;
using Game.GameVariant;
using Game.Quality;
using Game.Report;
using Game.Scene;
using Game.Sound;
using Game.UI;
using Game.UI.Form;
using Table;
using UnityEngine;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
using UnityEngine.iOS;
#endif

namespace Game.Utility
{
    /// <summary>
    /// 游戏开始时需要初始化的内容
    /// </summary>
    public static class GameStartUtility
    {
        public static async UniTask StartLoadAsync()
        {
            await InitResourceAsync();

            new GameDataSystem();
            await GameVariantLoadAsync();
            await GameStartUpAsync();
        }

        public static async UniTask InitResourceAsync()
        {
            if (!Framework.BaseComponent.EditorResourceMode)
            {
                var resourceInitComplete = false;
                Framework.ResourceComponent.InitResources(() => resourceInitComplete = true);

                await UniTask.WaitUntil(() => resourceInitComplete);
            }

            UnityGameFramework.Runtime.Log.Info("resource component init complete");
        }

        /// <summary>
        /// 包内AB Test系统载入
        /// </summary>
        public static async UniTask GameVariantLoadAsync()
        {
            var variantSetting =
                await ResourceUtility.LoadAssetAsync<GameVariantSetting>(
                    "Assets/Game/Variants/GameVariantSetting.asset",
                    false);
            if (variantSetting.CheckIsUseVariant(out var variantConfig))
            {
                var gameVariantSystem = new GameVariantSystem(variantConfig);
                await gameVariantSystem.LoadVariantAssetDataAsync();
            }
            else
            {
                new GameVariantSystem(null);
            }
        }

        public static async UniTask GameStartUpAsync()
        {
            if (Framework.EntityComponent != null)
            {
                Framework.EntityComponent.AddEntityGroup(EntityGroupName.Car, 10f, 5, 60, 0);
                Framework.EntityComponent.AddEntityGroup(EntityGroupName.Camera, 10f, 1, 60, 0);
                Framework.EntityComponent.AddEntityGroup(EntityGroupName.Cannon, 10f, 2, 60, 0);
                Framework.EntityComponent.AddEntityGroup(EntityGroupName.Ball, 10f, 2, 60, 0);
                Framework.EntityComponent.AddEntityGroup(EntityGroupName.Platform, 10f, 4, 60, 0);
            }

            if (Framework.UiComponent != null)
            {
                Log.Info($"UI分组共有{UiDepth.MaxDepthValue}个");
                for (var i = 0; i < UiDepth.MaxDepthValue; i++)
                {
                    Framework.UiComponent.AddUIGroup((i + 1).ToString(), i + 1);
                }
            }

            //通用update caller
            new GameObject($"{nameof(CommonUnityUpdateFunctionCaller)}", typeof(CommonUnityUpdateFunctionCaller));

            //settings
            await SettingUtility.LoadSettingsAsync();
            //load table
            await LoadTables();

            new SceneSystem();
            new EntitySystem();
            new UISystem();
            new CoroutineSystem();
            new LunarConsoleSystem();
            new QualitySystem();
            new SoundSystem();
            new SceneElementSystem();
        #if UNITY_EDITOR
            new DebugSystem();
        #endif

            await UIUtility.LoadGrayMaterialAsync();
            //load ui
            var fundamentalFormTasks = new List<UniTask>
            {
                UIUtility.OpenFormAsync(BlackForm.UniqueId),
            };
            await UniTask.WhenAll(fundamentalFormTasks);
            await SoundUtility.InitialLoadAsync();

        #if OCF_ADVERTISEMENT_ENABLE
            //广告先开始初始化
            new AdvertisementSystem();
            //需要等待请求att权限
        #if UNITY_IOS
            await WaitIOSATTStatusConfirmAsync();
        #endif
        #endif

            //打点，必须在att之后
        #if OCF_REPORT_ENABLE
            var reportEventSystem = new ReportEventSystem();
            //必须等待facebook初始化完成，不然没法打点。facebook初始化一般需要一帧
            while (!FB.IsInitialized)
            {
                await UniTask.Yield();
            }
         #if UNITY_IOS
            var authorizationTrackingStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            reportEventSystem.ReportATTStatus(authorizationTrackingStatus);
        #endif
        #endif
        }

        public static async UniTask LoadTables()
        {
            var tableLoadDictionary = new Dictionary<string, Action<byte[]>>
            {
                // {"BoostUpgrade", BoostUpgradeTable.Parse},
            };

            foreach (var kv in tableLoadDictionary)
            {
                var loadedTextAsset = await ResourceUtility.LoadAssetAsync<TextAsset>($"Table/{kv.Key}.bytes");
                kv.Value(loadedTextAsset.bytes);
                ResourceUtility.UnLoadAsset(loadedTextAsset);
            }
        }
        
    #if UNITY_IOS
        /// <summary>
        /// 等待att权限确定
        /// </summary>
        private static async UniTask WaitIOSATTStatusConfirmAsync()
        {
            //IOS 14.5以上需要检查
            var iosVersion = Version.Parse(Device.systemVersion);
            Log.Info($"ios version {iosVersion}");
            var needToCheckAttStatus = (iosVersion.Major == 14 && iosVersion.Minor >= 5) || iosVersion.Major > 14;
            if (!needToCheckAttStatus)
            {
                return;
            }

            //max插件会帮我们弹出att弹窗，只需要等待用户选择完成
            //用户选择接受、不接受、没选择，总之结果不是NOT_DETERMINED。att请求就结束了
            await UniTask.WaitUntil(() =>
                ATTrackingStatusBinding.GetAuthorizationTrackingStatus() !=
                ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED);
            var authorizationTrackingStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            Log.Info($"用户选择att完成，结果{authorizationTrackingStatus.ToString()}");

            //如果开启了打点，那么facebook的SetAdvertiserTrackingEnabled需要设置
        #if OCF_REPORT_ENABLE
            if (authorizationTrackingStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED)
            {
                FacebookAdSettings.SetAdvertiserTrackingEnabled(true);
            }
            else
            {
                FacebookAdSettings.SetAdvertiserTrackingEnabled(false);
            }
        #endif
        }
    #endif
    }
}
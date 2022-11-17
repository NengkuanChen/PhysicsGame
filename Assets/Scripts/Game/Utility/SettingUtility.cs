using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Advertisement;
using Game.GameSystem;
using Game.PlatForm;
using Game.Report;
using Game.Setting;
using UnityEngine;

namespace Game.Utility
{
    public static class SettingUtility
    {
        public static GameSetting GameSetting { get; private set; }
        public static QualitySetting QualitySetting { get; private set; }
        public static ReportSetting ReportSetting { get; private set; }
        public static AdvertisementSetting AdvertisementSetting { get; private set; }
        
        public static PlatformSetting PlatformSetting { get; private set; }

        public static PlayerInputSetting PlayerInputSetting { get; private set; }
        
        public static async UniTask LoadSettingsAsync()
        {
            var loadTasks = new List<UniTask<ScriptableObject>>
            {
                ResourceUtility.LoadAssetAsync<ScriptableObject>("Setting/GameSetting.asset"),
                ResourceUtility.LoadAssetAsync<ScriptableObject>("Setting/QualitySetting.asset"),
                ResourceUtility.LoadAssetAsync<ScriptableObject>("Setting/ReportSetting.asset"),
                ResourceUtility.LoadAssetAsync<ScriptableObject>("Setting/AdvertisementSetting.asset"),
                ResourceUtility.LoadAssetAsync<ScriptableObject>("Setting/PlatformSetting.asset"),
                ResourceUtility.LoadAssetAsync<ScriptableObject>("Setting/PlayerInputSetting.asset"),
            };

            var results = await UniTask.WhenAll(loadTasks);

            var index = 0;
            GameSetting = results[index++] as GameSetting;
            QualitySetting = results[index++] as QualitySetting;
            ReportSetting = results[index++] as ReportSetting;
            AdvertisementSetting = results[index++] as AdvertisementSetting;
            PlatformSetting = results[index++] as PlatformSetting;
            PlayerInputSetting = results[index++] as PlayerInputSetting;
        }
    }
}
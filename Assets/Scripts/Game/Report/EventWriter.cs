using System;
using System.Collections.Generic;
using Game.GameSystem;
using Game.GameVariant;
using Newtonsoft.Json;
using UnityEngine;
#if OCF_REPORT_ENABLE
using AppsFlyerSDK;
using Facebook.Unity;
#endif

namespace Game.Report
{
    public class EventWriter
    {
        private static string DeviceUniqueIdentifier => SystemInfo.deviceUniqueIdentifier;

        private Dictionary<string, string> appsflyerValues = new Dictionary<string, string>();
        private Dictionary<string, object> facebookValues = new Dictionary<string, object>();
        private string currentEventName;

        public void StartWrite(string eventName)
        {
            SetDefaultAppsflyersEventValue();
            SetDefaultFacebookEventValue();

            currentEventName = eventName;
        }

        public void AddParameter(string key, string value)
        {
        #if OCF_REPORT_ENABLE
            appsflyerValues.Add(key, value);
            facebookValues.Add(key, value);
        #endif
        }

        public void FlushEvent()
        {
            if (string.IsNullOrEmpty(currentEventName))
            {
                throw new Exception("无法发送事件,没有设置事件名称");
            }

        #if OCF_REPORT_ENABLE
            AppsFlyer.sendEvent(currentEventName, appsflyerValues);
            FB.LogAppEvent(currentEventName, null, facebookValues);
        #endif
            Log.LogEventReport(currentEventName, JsonConvert.SerializeObject(appsflyerValues));
            currentEventName = null;
        }

        private void SetDefaultAppsflyersEventValue()
        {
        #if OCF_REPORT_ENABLE
            appsflyerValues.Clear();
            var gameDataSystem = GameDataSystem.Get();
            if (gameDataSystem == null)
            {
                throw new GameLogicException($"{nameof(ReportEventSystem)} 需要 {nameof(GameDataSystem)}");
            }

            appsflyerValues.Add(ReportParameterName.GameLaunchTimes, gameDataSystem.GameLaunchTimes.ToString());
            appsflyerValues.Add(ReportParameterName.TotalGameSeconds, gameDataSystem.TotalGameTime.ToString("0.00"));
            appsflyerValues.Add(ReportParameterName.Version, Application.version);
            appsflyerValues.Add(ReportParameterName.GameVariantName, gameDataSystem.VariantName);
            appsflyerValues.Add(ReportParameterName.DeviceId, DeviceUniqueIdentifier);
        #endif
        }

        private void SetDefaultFacebookEventValue()
        {
        #if OCF_REPORT_ENABLE
            facebookValues.Clear();
            var gameDataSystem = GameDataSystem.Get();
            if (gameDataSystem == null)
            {
                throw new GameLogicException($"{nameof(ReportEventSystem)} 需要 {nameof(GameDataSystem)}");
            }

            facebookValues.Add(ReportParameterName.GameLaunchTimes, gameDataSystem.GameLaunchTimes.ToString());
            facebookValues.Add(ReportParameterName.TotalGameSeconds, gameDataSystem.TotalGameTime.ToString("0.00"));
            facebookValues.Add(ReportParameterName.Version, Application.version);
            facebookValues.Add(ReportParameterName.GameVariantName, gameDataSystem.VariantName);
            facebookValues.Add(ReportParameterName.DeviceId, DeviceUniqueIdentifier);
        #endif
        }
    }
}
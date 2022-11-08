using UnityEngine;

namespace Game.Report
{
    [CreateAssetMenu(fileName = "ReportSetting", menuName = "Settings/ReportSetting", order = 0)]
    public class ReportSetting : ScriptableObject
    {
        [SerializeField]
        private string appsFlyerDevKeyIOS;
        public string AppsFlyerDevKeyIOS => appsFlyerDevKeyIOS;
        [SerializeField]
        private string appsFlyerDevKeyAndroid;
        public string AppsFlyerDevKeyAndroid => appsFlyerDevKeyAndroid;
        [SerializeField]
        private string appsFlyerIosAppId;
        public string AppsFlyerIosAppId => appsFlyerIosAppId;

    #if UNITY_EDITOR

        public void SetAndroidKey(string key)
        {
            appsFlyerDevKeyAndroid = key;
        }

        public void SetIosKey(string key)
        {
            appsFlyerDevKeyIOS = key;
        }

        public void SetIosAppId(string id)
        {
            appsFlyerIosAppId = id;
        }
    #endif
    }
}
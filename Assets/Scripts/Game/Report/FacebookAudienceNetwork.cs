using System.Runtime.InteropServices;

namespace Game.Report
{
    public static class FacebookAdSettings
    {
    #if UNITY_IOS && !UNITY_EDITOR && OCF_ADVERTISEMENT_ENABLE
        [DllImport("__Internal")]
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);
    #endif

        public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
        {
        #if UNITY_IOS && !UNITY_EDITOR && OCF_ADVERTISEMENT_ENABLE
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
        #endif
        }
    }
}
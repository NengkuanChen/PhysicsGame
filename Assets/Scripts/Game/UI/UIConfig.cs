using System.Collections.Generic;

namespace Game.UI
{
    public static class UiDepth
    {
        private static int depth = 1;
        public static readonly int Lowest = depth++;
        public static readonly int Control = depth++;
        public static readonly int Common = depth++;
        public static readonly int Coin = depth++;
        public static readonly int Guide = depth++;
        public static readonly int Loading = depth++;
        public static readonly int Highest = depth++;

        public static int MaxDepthValue => depth - 1;
    }

    public static class UIConfig
    {
        public class UIAssetInfo
        {
            public string assetName;
            public int uiDepth;
            public bool coverFullScreen;
        }

        /// <summary>
        /// 注册面板
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="formAssetName"></param>
        /// <param name="depth"></param>
        /// <param name="coverFullScreen">是否覆盖全屏幕，也就是完全看不到这个UI后面的任何东西，这时候会隐藏主摄像机提升性能</param>
        public static void RegisterForm(int uniqueId, string formAssetName, int depth, bool coverFullScreen = false)
        {
            uiAssetInfoDic.Add(uniqueId,
            new UIAssetInfo
            {
                assetName = formAssetName,
                uiDepth = depth,
                coverFullScreen = coverFullScreen
            });
            // Debug.Log($"注册UI{formAssetName}");
        }

        private static Dictionary<int, UIAssetInfo> uiAssetInfoDic = new Dictionary<int, UIAssetInfo>();
        public static Dictionary<int, UIAssetInfo> UIAssetInfoDic => uiAssetInfoDic;

        public static bool GetUiAssetNameAndDepth(int formType, out string assetName, out int depth)
        {
            assetName = "ErrorForm";
            depth = 1;
            if (uiAssetInfoDic.TryGetValue(formType, out var assetInfo))
            {
                assetName = assetInfo.assetName;
                depth = assetInfo.uiDepth;
                return true;
            }

            return false;
        }
    }
}
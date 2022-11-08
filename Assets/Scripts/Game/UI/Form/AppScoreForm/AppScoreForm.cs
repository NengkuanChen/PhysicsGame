using System.Collections.Generic;
using DG.Tweening;
using Game.GameSystem;
using Game.Utility;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Form.AppScoreForm
{
    public class AppScoreForm : GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;
        
        static AppScoreForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(AppScoreForm), UiDepth.Common);
        }

        [SerializeField, Required]
        private RectTransform windowRoot;

        [SerializeField, Required]
        private List<Image> starImages = new List<Image>();

        [SerializeField, Required]
        private TextMeshProUGUI submitText;

        [SerializeField, Required]
        private TextMeshProUGUI cancelText;

        [SerializeField]
        private Color deActiveStarColor = Color.white;
        [SerializeField]
        private Color activeStarColor = Color.white;

        private int currentActiveStarCount;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            if (starImages.Count != 5)
            {
                throw new GameLogicException($"评价窗口的星星数量不正确： {starImages.Count}");
            }

            for (var i = 0; i < starImages.Count; i++)
            {
                var starIndex = i + 1;
                var starImage = starImages[i];
                UIUtility.AddEventListener(starImage.gameObject,
                    EventTriggerType.PointerClick,
                    arg0 => { SetActiveStarCount(starIndex); });
            }

            UIUtility.AddEventListener(cancelText.gameObject,
                EventTriggerType.PointerClick,
                baseEventData => { CloseSelf(); });
            UIUtility.AddEventListener(submitText.gameObject,
                EventTriggerType.PointerClick,
                arg0 =>
                {
                    if (currentActiveStarCount >= 4)
                    {
                        OpenStoreLink();
                    }

                    CloseSelf();
                });
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            SetActiveStarCount(0);

            windowRoot.localScale = Vector3.zero;
            windowRoot.DOScale(1f, .3f).SetEase(Ease.OutBack);

            // GameDataSystem.Get().IsAppRateFormShowed = true;
        }

        private void SetActiveStarCount(int count)
        {
            currentActiveStarCount = count;

            for (var i = 0; i < starImages.Count; i++)
            {
                var starImage = starImages[i];
                starImage.color = i + 1 <= count ? activeStarColor : deActiveStarColor;
            }
        }

        private void OpenStoreLink()
        {
            Log.Info("跳转到商店页");
            if (Application.isEditor)
            {
                return;
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                //init AndroidJavaClass
                var UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var Intent = new AndroidJavaClass("android.content.Intent");
                var Uri = new AndroidJavaClass("android.net.Uri");

                // get currentActivity
                var currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                var jstr_content =
                    new AndroidJavaObject("java.lang.String", $"market://details?id={Application.identifier}");
                var intent = new AndroidJavaObject("android.content.Intent",
                    Intent.GetStatic<AndroidJavaObject>("ACTION_VIEW"),
                    Uri.CallStatic<AndroidJavaObject>("parse", jstr_content));
                currentActivity.Call("startActivity", intent);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // var reportSetting = SettingUtility.ReportSetting;
                // var url =
                    // $"itms-apps://itunes.apple.com/cn/app/id{reportSetting.AppsFlyerIosAppId}?mt=8&action=write-review";
                // Application.OpenURL(url);
            }
        }
    }
}
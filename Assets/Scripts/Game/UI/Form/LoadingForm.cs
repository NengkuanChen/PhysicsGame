using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.UI.Form
{
    public class LoadingForm : GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;

        static LoadingForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(LoadingForm), UiDepth.Loading, coverFullScreen: true);
        }

        [SerializeField, Required]
        private TextMeshProUGUI progressText;
        private int currentProgress;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UpdateProgress(0, 0f);
        }

        public void UpdateProgress(int p, float duration = .3f)
        {
            p = Mathf.Clamp(p, 0, 100);
            if (duration <= 0f)
            {
                progressText.text = $"{p:00}%";
                currentProgress = p;
            }
            else
            {
                DOTween.Kill(progressText);
                DOVirtual.Float(currentProgress,
                             p,
                             duration,
                             value =>
                             {
                                 currentProgress = Mathf.RoundToInt(value);
                                 progressText.text = $"{currentProgress:00}%";
                             })
                         .SetTarget(progressText);
            }
        }
    }
}
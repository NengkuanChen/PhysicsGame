using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class BlackForm : GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;
        static BlackForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(BlackForm), UiDepth.Highest);
        }

        [SerializeField, Required]
        private Image blackImage;

        [SerializeField, Required]
        private GameObject blockGameObject;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            blackImage.enabled = false;
            blockGameObject.SetActive(false);
        }

        public void ShowBlackMask(float duration)
        {
            blackImage.enabled = true;
            if (duration <= 0f)
            {
                blackImage.color = Color.black;
            }
            else
            {
                DOTween.Kill(blackImage);
                blackImage.DOColor(Color.black, duration);
            }
        }

        public void HideBlackMask(float duration)
        {
            if (duration <= 0f)
            {
                blackImage.enabled = false;
            }
            else
            {
                DOTween.Kill(blackImage);
                blackImage.DOColor(Color.clear, duration).onComplete = () => { blackImage.enabled = false; };
            }
        }

        public void EnableBlock(bool isEnable)
        {
            blockGameObject.SetActive(isEnable);
        }
    }
}
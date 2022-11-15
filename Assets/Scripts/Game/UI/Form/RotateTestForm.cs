using UnityEngine;

namespace Game.UI.Form
{
    public class RotateTestForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;
        static RotateTestForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(BlackForm), UiDepth.Common);
        }

        [SerializeField] 
        private RectTransform testImage;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
        }
    }
}
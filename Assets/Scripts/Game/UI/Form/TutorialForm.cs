using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class TutorialForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;
        
        [SerializeField, LabelText("Return Button")]
        private Button returnButton;

        static TutorialForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(TutorialForm), UiDepth.Highest);
        }


        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            returnButton.onClick.AddListener(() =>
            {
                CloseSelf();
            });
        }
    }
}
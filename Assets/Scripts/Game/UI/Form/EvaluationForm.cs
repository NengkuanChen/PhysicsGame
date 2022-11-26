using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class EvaluationForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;

        [SerializeField, LabelText("Continue Button")]
        private Button continueButton;
        
        private bool hasFinished = false;
        public bool HasFinished => hasFinished;
        
        static EvaluationForm()
        {
            UIConfig.RegisterForm(UniqueId,  nameof(EvaluationForm), UiDepth.Common);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            continueButton.onClick.AddListener((() =>
            {
                hasFinished = true;
            }));
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            hasFinished = false;
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            hasFinished = true;
        }
    }
}
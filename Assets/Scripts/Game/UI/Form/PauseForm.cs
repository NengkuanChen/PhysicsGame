using Game.GameEvent;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class PauseForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;
        
        [SerializeField, LabelText("Resume Button")]
        private Button resumeButton;

        static PauseForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(PauseForm), UiDepth.Common);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            resumeButton.onClick.AddListener(() =>
            {
                Framework.EventComponent.Fire(this, OnGamePauseEventArgs.Create(false));
            });
        }
    }
}
using Game.GameEvent;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class WaitingStartForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();

        public override int FormType => UniqueId;

        [SerializeField, LabelText("Start Button"), Required]
        private Button startButton;
        
        static WaitingStartForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(WaitingStartForm), UiDepth.Common);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            startButton.onClick.AddListener((() =>
            {
                Framework.EventComponent.Fire(this, OnGameStartEventArgs.Create());
                startButton.interactable = false;
                CloseSelf();
            }));
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            startButton.interactable = true;
        }
    }
}
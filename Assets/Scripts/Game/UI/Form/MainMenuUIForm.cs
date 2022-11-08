using Game.GameEvent;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


namespace Game.UI.Form
{
    public class MainMenuUIForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;

        static MainMenuUIForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(MainMenuUIForm), UiDepth.Common, coverFullScreen: true);
        }

        [SerializeField, LabelText("开始按钮"), Required]
        private Button startButton;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            startButton.onClick.AddListener(()=> Framework.EventComponent.Fire(this,OnGameStartEventArgs.Create()));
        }
    }
}
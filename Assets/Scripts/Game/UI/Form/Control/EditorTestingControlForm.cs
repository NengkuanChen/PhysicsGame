using Game.GameEvent;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form.Control
{
    public class EditorTestingControlForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;

        [SerializeField] 
        private Slider controlSlider;

        static EditorTestingControlForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(EditorTestingControlForm), UiDepth.Common);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            controlSlider.onValueChanged.AddListener(controlValue =>
            {
                Framework.EventComponent.Fire(this, OnEditorPlayerMoveBallEventArgs.Create(controlValue));
            });
        }
    }
}
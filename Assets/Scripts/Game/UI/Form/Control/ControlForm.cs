using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form.Control
{
    public class ControlForm : GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;

        [SerializeField, LabelText("Control Area")] 
        private Image controlArea;

        static ControlForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(ControlForm), UiDepth.Control);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
        }
    }
}
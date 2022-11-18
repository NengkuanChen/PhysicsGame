using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form.Control
{
    public class ControlForm : GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;
        
        [SerializeField, Required, LabelText("Ball Switch Buttons")]
        private List<Button> ballSwitchButtons;

        static ControlForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(ControlForm), UiDepth.Control);
        }
        
    }
}
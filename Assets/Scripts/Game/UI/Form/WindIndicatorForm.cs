using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game.UI.Form
{
    public class WindIndicatorForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;
        
        [SerializeField, LabelText("Wind Indicator")] 
        private RectTransform windIndicator;
        
        static WindIndicatorForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(WindIndicatorForm), UiDepth.Common);
        }

        public void SetWindDirection(Vector3 direction)
        {
            windIndicator.eulerAngles = direction;
        }
    }
}
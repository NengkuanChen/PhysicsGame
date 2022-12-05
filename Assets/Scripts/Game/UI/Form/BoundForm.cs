using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI.Form
{
    public class BoundForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;

        [SerializeField, LabelText("LeftBound")]
        private RectTransform leftBound;
        public RectTransform LeftBound => leftBound;
        
        [SerializeField, LabelText("RightBound")]
        private RectTransform rightBound;
        public RectTransform RightBound => rightBound;
        
        [SerializeField, LabelText("TopBound")]
        private RectTransform topBound;
        public RectTransform TopBound => topBound;
        
        [SerializeField, LabelText("BottomBound")]
        private RectTransform bottomBound;
        public RectTransform BottomBound => bottomBound;

        static BoundForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(BoundForm), UiDepth.Lowest);
        }
    }
}
using Game.Ball;
using Game.GameEvent;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.Form.Control
{
    public class BallSwitchButtons: StaticUIElement, IPointerClickHandler
    {
        
        [SerializeField, LabelText("Ball Type")]
        private BallType ballType;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Framework.EventComponent.Fire(this, OnBallSwitchEventArgs.Create(ballType));
        }
    }
}
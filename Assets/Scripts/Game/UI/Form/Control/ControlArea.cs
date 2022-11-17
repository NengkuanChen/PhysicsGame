using Game.GameEvent;
using UnityEngine.EventSystems;

namespace Game.UI.Form.Control
{
    public class ControlArea: StaticUIElement, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            Framework.EventComponent.Fire(this, OnControlFormHitEventArgs.Create());
        }
    }
}
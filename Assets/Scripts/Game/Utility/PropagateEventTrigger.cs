using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Utility
{
    /// <summary>
    /// 对于没有注册的事件,将事件往父GameObject传递
    /// </summary>
    public class PropagateEventTrigger : EventTrigger
    {
        public bool isStopAllEventPropagate;

        private GameObject GetParentGameObject()
        {
            if (transform == null || transform.parent == null)
            {
                return null;
            }

            return transform.parent.gameObject;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.PointerDown))
            {
                base.OnPointerEnter(eventData);
            }
            else if (transform != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.pointerEnterHandler);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.PointerExit))
            {
                base.OnPointerExit(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.pointerExitHandler);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.Drag))
            {
                base.OnDrag(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.dragHandler);
            }
        }

        public override void OnDrop(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.Drop))
            {
                base.OnDrop(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.dropHandler);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.PointerDown))
            {
                base.OnPointerDown(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.pointerDownHandler);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.PointerUp))
            {
                base.OnPointerUp(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.pointerUpHandler);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.PointerClick))
            {
                base.OnPointerClick(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.pointerClickHandler);
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.Select))
            {
                base.OnSelect(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.selectHandler);
            }
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.Deselect))
            {
                base.OnDeselect(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.deselectHandler);
            }
        }

        public override void OnScroll(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.Scroll))
            {
                base.OnScroll(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.scrollHandler);
            }
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.Move))
            {
                base.OnMove(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.moveHandler);
            }
        }

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.UpdateSelected))
            {
                base.OnUpdateSelected(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.updateSelectedHandler);
            }
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.InitializePotentialDrag))
            {
                base.OnInitializePotentialDrag(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.initializePotentialDrag);
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.BeginDrag))
            {
                base.OnBeginDrag(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.beginDragHandler);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.EndDrag))
            {
                base.OnEndDrag(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.endDragHandler);
            }
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.Submit))
            {
                base.OnSubmit(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.submitHandler);
            }
        }

        public override void OnCancel(BaseEventData eventData)
        {
            if (CheckIsEventTriggerTypeRegister(EventTriggerType.Cancel))
            {
                base.OnCancel(eventData);
            }
            else if (GetParentGameObject() != null && !isStopAllEventPropagate)
            {
                ExecuteEvents.ExecuteHierarchy(GetParentGameObject(), eventData, ExecuteEvents.cancelHandler);
            }
        }

        private bool CheckIsEventTriggerTypeRegister(EventTriggerType type)
        {
            if (triggers.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < triggers.Count; i++)
            {
                Entry entry = triggers[i];
                if (entry.eventID == type)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
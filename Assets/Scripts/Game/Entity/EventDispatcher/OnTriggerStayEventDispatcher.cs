using UnityEngine;

namespace Game.Entity.EventDispatcher
{
    public class OnTriggerStayEventDispatcher : OnTriggerEventDispatcherBase
    {
        private void OnTriggerStay(Collider other)
        {
            InvokeListeners(other);
        }
    }
}
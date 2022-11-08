using UnityEngine;

namespace Game.Entity.EventDispatcher
{
    public class OnTriggerExitEventDispatcher : OnTriggerEventDispatcherBase
    {
        private void OnTriggerExit(Collider other)
        {
            InvokeListeners(other);
        }
    }
}
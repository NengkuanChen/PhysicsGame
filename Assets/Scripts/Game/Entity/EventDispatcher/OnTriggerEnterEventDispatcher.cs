using UnityEngine;

namespace Game.Entity.EventDispatcher
{
    public class OnTriggerEnterEventDispatcher : OnTriggerEventDispatcherBase
    {
        private void OnTriggerEnter(Collider other)
        {
            InvokeListeners(other);
        }
    }
}
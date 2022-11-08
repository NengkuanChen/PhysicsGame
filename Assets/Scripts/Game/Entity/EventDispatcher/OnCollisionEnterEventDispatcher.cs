using UnityEngine;

namespace Game.Entity.EventDispatcher
{
    public class OnCollisionEnterEventDispatcher : OnCollisionEventDispatcherBase
    {
        private void OnCollisionEnter(Collision other)
        {
            InvokeListeners(other);
        }
    }
}
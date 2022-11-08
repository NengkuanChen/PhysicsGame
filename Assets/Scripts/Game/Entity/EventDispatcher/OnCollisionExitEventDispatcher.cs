using UnityEngine;

namespace Game.Entity.EventDispatcher
{
    public class OnCollisionExitEventDispatcher : OnCollisionEventDispatcherBase
    {
        private void OnCollisionExit(Collision other)
        {
            InvokeListeners(other);
        }
    }
}
using UnityEngine;

namespace Game.Entity.EventDispatcher
{
    public class OnCollisionStayEventDispatcher : OnCollisionEventDispatcherBase
    {
        private void OnCollisionStay(Collision other)
        {
            InvokeListeners(other);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity.EventDispatcher
{
    public abstract class OnCollisionEventDispatcherBase : EntityComponentUnityCallbackEventDispatcherBase
    {
        protected Rigidbody rigidbody;
        protected LinkedList<Action<Rigidbody, Collision>> listeners = new LinkedList<Action<Rigidbody, Collision>>();

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public void RegisterListener(Action<Rigidbody, Collision> callback)
        {
            listeners.AddLast(callback);
        }
        
        private void OnDisable()
        {
            var node = listeners.First;
            while (node != null)
            {
                var next = node.Next;
                if (node.Value == null)
                {
                    listeners.Remove(node);
                }

                node = next;
            }
            if (listeners.Count == 0)
            {
                Destroy(this);
            }
        }

        public void UnRegisterListener(Action<Rigidbody, Collision> callback)
        {
            var node = listeners.First;
            while (node != null)
            {
                if (node.Value == callback)
                {
                    node.Value = null;
                    break;
                }

                node = node.Next;
            }
        }

        protected void InvokeListeners(Collision collision)
        {
            var node = listeners.First;
            while (node != null)
            {
                var nextNode = node.Next;
                if (node.Value != null)
                {
                    node.Value.Invoke(rigidbody, collision);
                }
                else
                {
                    listeners.Remove(node);
                }

                node = nextNode;
            }
        }
    }
}
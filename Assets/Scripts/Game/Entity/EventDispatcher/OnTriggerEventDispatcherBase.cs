using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity.EventDispatcher
{
    public abstract class OnTriggerEventDispatcherBase : EntityComponentUnityCallbackEventDispatcherBase
    {
        protected Rigidbody rigidbody;
        protected LinkedList<Action<Rigidbody, Collider>> listeners = new LinkedList<Action<Rigidbody, Collider>>();

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
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

        public void RegisterListener(Action<Rigidbody, Collider> callback)
        {
            listeners.AddLast(callback);
        }

        public void UnRegisterListener(Action<Rigidbody, Collider> callback)
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

        protected void InvokeListeners(Collider collider)
        {
            var node = listeners.First;
            while (node != null)
            {
                var nextNode = node.Next;
                if (node.Value != null)
                {
                    node.Value.Invoke(rigidbody, collider);
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
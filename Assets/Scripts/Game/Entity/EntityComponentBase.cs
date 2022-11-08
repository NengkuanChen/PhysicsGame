using System;
using Game.Entity.EventDispatcher;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using Object = System.Object;

namespace Game.Entity
{
    public abstract class EntityComponentBase
    {
        public abstract int ID { get; }
        /// <summary>
        /// 值越小 优先级越高
        /// </summary>
        public virtual int Priority => EntityComponentPriority.Common;

        public virtual bool NeedLateUpdate => false;
        public virtual bool NeedFixedUpdate => false;

        public bool IsCreatedFromReferencePool { get; set; }

        protected virtual GameEntityLogic OwnerEntity { get; set; }
        protected Transform OwnerTransform => OwnerEntity.CachedTransform;

        public bool Available { get; protected set; }

        public void SetEntityLogic(GameEntityLogic e)
        {
            OwnerEntity = e;
        }

        public virtual void OnComponentAttach()
        {
            Available = true;
        #if UNITY_EDITOR
            if (this is IReference && !IsCreatedFromReferencePool)
            {
                throw new Exception($"组件 {GetType().FullName} 需要从ReferencePool中构建");
            }
        #endif
        }

        public virtual void OnComponentDetach()
        {
            Available = false;
        }

        public virtual void OnEntityUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        public virtual void OnEntityLateUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        public virtual void OnEntityFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        protected void RemoveSelf()
        {
            if (!Available)
            {
                return;
            }

            if (OwnerEntity != null)
            {
                OwnerEntity.RemoveComponent(this);
            }
        }

        protected void AddEntityComponent(EntityComponentBase component)
        {
            OwnerEntity.AddComponent(component);
        }

        protected void RemoveEntityComponent(EntityComponentBase component)
        {
            OwnerEntity.RemoveComponent(component);
        }

        protected void RemoveEntityComponent(int componentID)
        {
            OwnerEntity.RemoveComponent(componentID);
        }

        protected bool IsEntityContainComponent(int componentID)
        {
            return OwnerEntity.ContainComponent(componentID);
        }

        protected K GetEntityComponent<K>(int componentID) where K : EntityComponentBase
        {
            var result = OwnerEntity.GetComponent(componentID) as K;
            return result;
        }

        protected void SubscribeInnerEvent(int eventId, EventHandler<GameEventArgs> e)
        {
            OwnerEntity.SubscribeEntityInnerEvent(eventId, e);
        }

        public void UnSubscribeInnerEvent(int eventId, EventHandler<GameEventArgs> e)
        {
            OwnerEntity.UnSubscribeEntityInnerEvent(eventId, e);
        }

        protected void FireInnerEvent(Object sender, GameEventArgs e)
        {
            OwnerEntity.FireComponentInnerEvent(sender, e);
        }

        private void RegisterCollisionEvent<T>(Rigidbody rigidbody, Action<Rigidbody, Collision> callback)
            where T : OnCollisionEventDispatcherBase
        {
            if (rigidbody == null)
            {
                return;
            }

            var eventDispatcher = rigidbody.gameObject.GetOrAddComponent<T>();
            eventDispatcher.RegisterListener(callback);
        }

        private void UnRegisterCollisionEvent<T>(Rigidbody rigidbody, Action<Rigidbody, Collision> callback)
            where T : OnCollisionEventDispatcherBase
        {
            if (rigidbody == null)
            {
                return;
            }

            var eventDispatcher = rigidbody.gameObject.GetComponent<T>();
            if (eventDispatcher != null)
            {
                eventDispatcher.UnRegisterListener(callback);
            }
        }

        protected void RegisterOnCollisionEnterEvent(Rigidbody rigidbody, Action<Rigidbody, Collision> callback)
        {
            RegisterCollisionEvent<OnCollisionEnterEventDispatcher>(rigidbody, callback);
        }

        protected void UnRegisterOnCollisionEnterEvent(Rigidbody rigidbody, Action<Rigidbody, Collision> callback)
        {
            UnRegisterCollisionEvent<OnCollisionEnterEventDispatcher>(rigidbody, callback);
        }

        protected void RegisterOnCollisionExitEvent(Rigidbody rigidbody, Action<Rigidbody, Collision> callback)
        {
            RegisterCollisionEvent<OnCollisionExitEventDispatcher>(rigidbody, callback);
        }

        protected void UnRegisterOnCollisionExitEvent(Rigidbody rigidbody, Action<Rigidbody, Collision> callback)
        {
            UnRegisterCollisionEvent<OnCollisionExitEventDispatcher>(rigidbody, callback);
        }

        protected void RegisterOnCollisionStayEvent(Rigidbody rigidbody, Action<Rigidbody, Collision> callback)
        {
            RegisterCollisionEvent<OnCollisionStayEventDispatcher>(rigidbody, callback);
        }

        protected void UnRegisterOnCollisionStayEvent(Rigidbody rigidbody, Action<Rigidbody, Collision> callback)
        {
            UnRegisterCollisionEvent<OnCollisionStayEventDispatcher>(rigidbody, callback);
        }

        private void RegisterTriggerEvent<T>(Rigidbody rigidbody, Action<Rigidbody, Collider> callback)
            where T : OnTriggerEventDispatcherBase
        {
            if (rigidbody == null)
            {
                return;
            }

            var eventDispatcher = rigidbody.gameObject.GetOrAddComponent<T>();
            eventDispatcher.RegisterListener(callback);
        }

        private void UnRegisterTriggerEvent<T>(Rigidbody rigidbody, Action<Rigidbody, Collider> callback)
            where T : OnTriggerEventDispatcherBase
        {
            if (rigidbody == null)
            {
                return;
            }

            var eventDispatcher = rigidbody.gameObject.GetComponent<T>();
            if (eventDispatcher != null)
            {
                eventDispatcher.UnRegisterListener(callback);
            }
        }

        protected void RegisterOnTriggerEnterEvent(Rigidbody rigidbody, Action<Rigidbody, Collider> callback)
        {
            RegisterTriggerEvent<OnTriggerEnterEventDispatcher>(rigidbody, callback);
        }

        protected void UnRegisterOnTriggerEnterEvent(Rigidbody rigidbody, Action<Rigidbody, Collider> callback)
        {
            UnRegisterTriggerEvent<OnTriggerEnterEventDispatcher>(rigidbody, callback);
        }

        protected void RegisterOnTriggerExitEvent(Rigidbody rigidbody, Action<Rigidbody, Collider> callback)
        {
            RegisterTriggerEvent<OnTriggerExitEventDispatcher>(rigidbody, callback);
        }

        protected void UnRegisterOnTriggerExitEvent(Rigidbody rigidbody, Action<Rigidbody, Collider> callback)
        {
            UnRegisterTriggerEvent<OnTriggerExitEventDispatcher>(rigidbody, callback);
        }
        
        protected void RegisterOnTriggerStayEvent(Rigidbody rigidbody, Action<Rigidbody, Collider> callback)
        {
            RegisterTriggerEvent<OnTriggerStayEventDispatcher>(rigidbody, callback);
        }

        protected void UnRegisterOnTriggerStayEvent(Rigidbody rigidbody, Action<Rigidbody, Collider> callback)
        {
            UnRegisterTriggerEvent<OnTriggerStayEventDispatcher>(rigidbody, callback);
        }

    #if UNITY_EDITOR
        /// <summary>
        /// 游戏运行时,组件绘制gizmos
        /// </summary>
        public virtual void OnDrawGizmos()
        {
        }

        /// <summary>
        /// 运行时 绘制GUI
        /// </summary>
        public virtual void OnGUI()
        {
        }
    #endif
    }
}
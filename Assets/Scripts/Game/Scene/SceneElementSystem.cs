using System;
using System.Collections.Generic;
using Game.Common;
using Game.GameSystem;

namespace Game.Scene
{
    /// <summary>
    /// 处理场景中所有element
    /// </summary>
    public class SceneElementSystem : SystemBase
    {
        public static SceneElementSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as SceneElementSystem;
        }

        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private static Type sceneElementEventInterfaceType = typeof(ISceneElementEventInterface);

        /// <summary>
        /// 每种事件对应interface的element链表
        /// </summary>
        private Dictionary<Type, LinkedList<SceneElement>> eventToElementMap =
            new Dictionary<Type, LinkedList<SceneElement>>();

        private bool haveElementNeedRemove;

        internal override void OnEnable()
        {
            base.OnEnable();

            CommonUnityUpdateFunctionCaller.RegisterFixedUpdateCall(OnFixedUpdate);
            CommonUnityUpdateFunctionCaller.RegisterLateUpdateCall(OnLateUpdate);
        }

        public void SubscribeElement(SceneElement sceneElement)
        {
            if (sceneElement == null)
            {
                return;
            }

            if (sceneElement.isDestroyed)
            {
                throw new Exception($"{nameof(SceneElement)} {sceneElement.name} 已经被销毁，但是仍然在注册自己");
            }

            var elementType = sceneElement.GetType();
            var interfaceTypes = elementType.GetInterfaces();
            if (interfaceTypes.Length > 0)
            {
                for (var i = 0; i < interfaceTypes.Length; i++)
                {
                    var interfaceType = interfaceTypes[i];
                    if (interfaceType == sceneElementEventInterfaceType)
                    {
                        continue;
                    }

                    if (!(sceneElementEventInterfaceType.IsAssignableFrom(interfaceType)))
                    {
                        continue;
                    }

                    if (eventToElementMap.TryGetValue(interfaceType, out var linkedList))
                    {
                        linkedList.AddLast(sceneElement);
                    }
                    else
                    {
                        var newLinkedList = new LinkedList<SceneElement>();
                        newLinkedList.AddLast(sceneElement);
                        eventToElementMap.Add(interfaceType, newLinkedList);
                    }
                }
            }
        }

        public void UnSubscribeElement(SceneElement sceneElement)
        {
            haveElementNeedRemove = true;
        }

        private List<ISceneElementUpdate> cacheUpdateSceneElements = new List<ISceneElementUpdate>();
        private List<ISceneElementFixedUpdate> cacheFixedUpdateSceneElements = new List<ISceneElementFixedUpdate>();
        private List<ISceneElementLateUpdate> cacheLateUpdateSceneElements = new List<ISceneElementLateUpdate>();

        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            Internal_FilterElementByInterfaceNonAlloc(cacheUpdateSceneElements);
            if (cacheUpdateSceneElements.Count > 0)
            {
                for (var i = 0; i < cacheUpdateSceneElements.Count; i++)
                {
                    cacheUpdateSceneElements[i].OnUpdate(elapseSeconds, realElapseSeconds);
                }
            }

            if (haveElementNeedRemove)
            {
                RemoveDestroyedElement();
            }
        }

        private void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            Internal_FilterElementByInterfaceNonAlloc(cacheFixedUpdateSceneElements);
            if (cacheFixedUpdateSceneElements.Count > 0)
            {
                for (var i = 0; i < cacheFixedUpdateSceneElements.Count; i++)
                {
                    cacheFixedUpdateSceneElements[i].OnFixedUpdate(elapseSeconds, realElapseSeconds);
                }
            }
        }

        private void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {
            Internal_FilterElementByInterfaceNonAlloc(cacheLateUpdateSceneElements);
            if (cacheLateUpdateSceneElements.Count > 0)
            {
                for (var i = 0; i < cacheLateUpdateSceneElements.Count; i++)
                {
                    cacheLateUpdateSceneElements[i].OnLateUpdate(elapseSeconds, realElapseSeconds);
                }
            }
        }

        public static void FilterElementByInterfaceNonAlloc<T>(List<T> results) where T : class
        {
            var currentSystem = Get();
            currentSystem.Internal_FilterElementByInterfaceNonAlloc(results);
        }

        private void Internal_FilterElementByInterfaceNonAlloc<T>(List<T> results) where T : class
        {
            if (results == null)
            {
                return;
            }

            results.Clear();

            var interfaceType = typeof(T);
            if (eventToElementMap.TryGetValue(interfaceType, out var linkedList) && linkedList.Count > 0)
            {
                var current = linkedList.First;
                while (current != null)
                {
                    var next = current.Next;
                    if (!current.Value.isDestroyed)
                    {
                        var instance = current.Value as T;
                        if (instance == null)
                        {
                            throw new Exception($"类型{current.Value.GetType().FullName}没有继承自接口{interfaceType.FullName}");
                        }

                        results.Add(instance);
                    }

                    current = next;
                }
            }
        }

        public static void FilterElementByInterface<T>(Action<T> actionOnInterface) where T : class
        {
            var currentSystem = Get();
            currentSystem.Internal_FilterElementByInterface(actionOnInterface);
        }

        private void Internal_FilterElementByInterface<T>(Action<T> actionOnInterface) where T : class
        {
            var interfaceType = typeof(T);
            if (eventToElementMap.TryGetValue(interfaceType, out var linkedList) && linkedList.Count > 0)
            {
                var current = linkedList.First;
                while (current != null)
                {
                    var next = current.Next;
                    if (!current.Value.isDestroyed)
                    {
                        var instance = current.Value as T;
                        if (instance == null)
                        {
                            throw new Exception($"类型{current.Value.GetType().FullName}没有继承自接口{interfaceType.FullName}");
                        }

                        actionOnInterface.Invoke(instance);
                    }

                    current = next;
                }
            }
        }

        private void RemoveDestroyedElement()
        {
            haveElementNeedRemove = false;

            foreach (var kv in eventToElementMap)
            {
                var sceneElements = kv.Value;
                if (sceneElements.Count > 0)
                {
                    var current = sceneElements.First;
                    while (current != null)
                    {
                        var next = current.Next;
                        if (current.Value.isDestroyed)
                        {
                            sceneElements.Remove(current);
                        }

                        current = next;
                    }
                }
            }
        }
    }
}
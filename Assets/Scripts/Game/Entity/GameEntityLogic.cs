using System;
using System.Collections.Generic;
using Game.GameEvent;
using GameFramework;
using GameFramework.Event;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;
using UnityGameFramework.Runtime;
using Object = System.Object;
using System.IO;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;

#endif

namespace Game.Entity
{
    public class GameEntityLogic : EntityLogic
    {
        private bool ComponentContentInit => componentDic != null;

        private LinkedList<EntityComponentBase> runningComponentLinkList;
        private Dictionary<int, EntityComponentBase> componentDic;
        private bool componentListChanged;

        private EntityNativeArrayPool nativeArrayPool;

    #if UNITY_EDITOR

        [Serializable]
        private class ComponentInfo
        {
            private EntityComponentBase component;
            public EntityComponentBase Component => component;
            private Action<EntityComponentBase> removeFunction;
            [SerializeField, HorizontalGroup, Sirenix.OdinInspector.ReadOnly, HideLabel]
            private string componentName;
            public string ComponentName => componentName;

            public ComponentInfo(EntityComponentBase component, Action<EntityComponentBase> removeFunction)
            {
                this.component = component;
                this.removeFunction = removeFunction;
                componentName = component.GetType().Name;
            }

            public ComponentInfo(ComponentInfo copyInfo)
            {
                component = copyInfo.component;
                removeFunction = copyInfo.removeFunction;
                componentName = copyInfo.componentName;
            }

            [Button(ButtonSizes.Small, Name = "X"), HorizontalGroup]
            private void Remove()
            {
                removeFunction?.Invoke(component);
            }

            [Button(ButtonSizes.Small, Name = "open"), HorizontalGroup]
            private void OpenComponentScriptFile()
            {
                var assetsGuids = AssetDatabase.FindAssets("t:script", new[] {"Assets/Scripts/Game"});
                foreach (var guid in assetsGuids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var fileName = Path.GetFileNameWithoutExtension(assetPath);
                    if (string.Equals(fileName, componentName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath));
                        return;
                    }
                }
            }
        }

        private bool IsSearchingComponent => !string.IsNullOrEmpty(searchingComponentName);
        [BoxGroup("组件"), ShowInInspector, LabelText("搜索"), OnValueChanged("OnSearchingTextChanged"),
         ShowIf("WillShowSearchingText")]
        private string searchingComponentName;
        [BoxGroup("组件"), ShowInInspector, HideReferenceObjectPicker, ShowIf("WillShowComponentInfoList"),
         LabelText("当前组件"),
         ListDrawerSettings(Expanded = true,
             HideAddButton = true,
             HideRemoveButton = true,
             DraggableItems = false,
             ShowIndexLabels = false,
             IsReadOnly = true)]
        private List<ComponentInfo> sortedComponentInfos;
        [BoxGroup("组件"), ShowInInspector, HideReferenceObjectPicker, ShowIf("WillShowSearchingComponentList"),
         LabelText("搜索组件..."),
         ListDrawerSettings(Expanded = true,
             HideAddButton = true,
             HideRemoveButton = true,
             DraggableItems = false,
             ShowIndexLabels = false,
             IsReadOnly = true)]
        private List<ComponentInfo> searchingComponentInfos;

        private bool WillShowSearchingText()
        {
            return sortedComponentInfos != null && sortedComponentInfos.Count > 0;
        }

        private bool WillShowComponentInfoList()
        {
            return !IsSearchingComponent && sortedComponentInfos != null && sortedComponentInfos.Count > 0;
        }

        private bool WillShowSearchingComponentList()
        {
            return IsSearchingComponent;
        }

        private class ComponentSearchingResult
        {
            public ComponentInfo componentInfo;
            public int score;
        }

        private void OnSearchingTextChanged()
        {
            searchingComponentInfos.Clear();
            var searchingResults = new List<ComponentSearchingResult>();
            foreach (var info in sortedComponentInfos)
            {
                var componentName = info.ComponentName;
                if (FuzzySearch.Contains(searchingComponentName, componentName, out var score))
                {
                    searchingResults.Add(new ComponentSearchingResult
                    {
                        componentInfo = info,
                        score = score
                    });
                }
            }

            if (searchingResults.Count > 0)
            {
                searchingResults.Sort((a, b) => b.score - a.score);
                foreach (var result in searchingResults)
                {
                    searchingComponentInfos.Add(new ComponentInfo(result.componentInfo));
                }
            }
        }
    #endif

        private bool lateUpdateEnabled;
        private bool fixedUpdateEnabled;

        public override void OnInit(object userData)
        {
            if (m_CachedTransform == null)
            {
                m_CachedTransform = transform;
            }

            m_Entity = GetComponent<UnityGameFramework.Runtime.Entity>();
            m_OriginalTransform = CachedTransform.parent;
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            if (isShutdown)
            {
                ReleaseAllNativeArray();
                return;
            }

            Visible = false;
            m_Available = false;

            CachedTransform.SetParent(m_OriginalTransform);
            RemoveAllComponents();
            componentEventPool.Clear();
        }

        private void LoopRunningComponentLinkList(Action<EntityComponentBase, float, float> runningMethod,
                                                  float deltaTime,
                                                  float unscaleDeltaTime,
                                                  out bool haveComponentBeRemoved)
        {
            haveComponentBeRemoved = false;
            var currentNode = runningComponentLinkList.First;
            while (currentNode != null)
            {
                if (!currentNode.Value.Available)
                {
                    haveComponentBeRemoved = true;
                    //remove this
                    var deleteNode = currentNode;
                    // Log.Info($"remove component {deleteNode.Value.GetType().Name}");
                    currentNode = currentNode.Next;
                    deleteNode.Value.SetEntityLogic(null);
                    runningComponentLinkList.Remove(deleteNode);
                    TryReleaseReferenceComponent(deleteNode.Value);
                }
                else
                {
                    runningMethod?.Invoke(currentNode.Value, deltaTime, unscaleDeltaTime);
                    currentNode = currentNode.Next;
                }
            }
        }

        private static Action<EntityComponentBase, float, float> componentUpdateAction;

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (ComponentContentInit && Available)
            {
                componentUpdateAction ??= (component, deltaTime, unscaleDeltaTime) =>
                {
                    component.OnEntityUpdate(deltaTime, unscaleDeltaTime);
                };

                LoopRunningComponentLinkList(componentUpdateAction,
                    elapseSeconds,
                    realElapseSeconds,
                    out var haveComponentBeRemoved);
                componentListChanged |= haveComponentBeRemoved;

                if (componentListChanged)
                {
                #if UNITY_EDITOR
                    RefreshComponentInfos();
                #endif

                    var needLateUpdate = false;
                    var needFixedUpdate = false;
                    foreach (var component in runningComponentLinkList)
                    {
                        if (!component.Available)
                        {
                            continue;
                        }

                        needLateUpdate = needLateUpdate || component.NeedLateUpdate;
                        needFixedUpdate = needFixedUpdate || component.NeedFixedUpdate;
                    }

                    if (needLateUpdate != lateUpdateEnabled)
                    {
                        lateUpdateEnabled = needLateUpdate;
                        if (lateUpdateEnabled)
                        {
                            Framework.AddLateUpdateEntity(this);
                        }
                        else
                        {
                            Framework.RemoveLateUpdateEntity(this);
                        }
                    }

                    if (needFixedUpdate != fixedUpdateEnabled)
                    {
                        fixedUpdateEnabled = needFixedUpdate;
                        if (fixedUpdateEnabled)
                        {
                            Framework.AddFixedUpdateEntity(this);
                        }
                        else
                        {
                            Framework.RemoveFixedUpdateEntity(this);
                        }
                    }
                }
            }

            componentListChanged = false;
        }

        private static Action<EntityComponentBase, float, float> componentLateUpdateAction;

        public void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {
            componentLateUpdateAction ??= (component, deltaTime, unscaleDeltaTime) =>
            {
                if (component.NeedLateUpdate)
                {
                    component.OnEntityLateUpdate(deltaTime, unscaleDeltaTime);
                }
            };

            if (Available && ComponentContentInit)
            {
                LoopRunningComponentLinkList(componentLateUpdateAction,
                    elapseSeconds,
                    realElapseSeconds,
                    out var componentBeRemoved);
                componentListChanged |= componentBeRemoved;
            }
        }

        private static Action<EntityComponentBase, float, float> componentFixedUpdateAction;

        public void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            componentFixedUpdateAction ??= (component, deltaTime, unscaleDeltaTime) =>
            {
                if (component.NeedFixedUpdate)
                {
                    component.OnEntityFixedUpdate(deltaTime, unscaleDeltaTime);
                }
            };

            if (Available && ComponentContentInit)
            {
                LoopRunningComponentLinkList(componentFixedUpdateAction,
                    elapseSeconds,
                    realElapseSeconds,
                    out var componentBeRemoved);
                componentListChanged |= componentBeRemoved;
            }
        }

        public void AddComponent(EntityComponentBase component)
        {
            if (!ComponentContentInit)
            {
                runningComponentLinkList = new LinkedList<EntityComponentBase>();
                componentDic = new Dictionary<int, EntityComponentBase>();
            #if UNITY_EDITOR
                sortedComponentInfos = new List<ComponentInfo>();
                searchingComponentInfos = new List<ComponentInfo>();
            #endif
            }

        #if UNITY_EDITOR
            if (ContainComponent(component.ID))
            {
                Log.Error($"{name} entity重复添加component: {component.GetType().FullName}");
                return;
            }
        #endif

            if (!Available)
            {
                return;
            }

            var currentNode = runningComponentLinkList.First;
            while (true)
            {
                if (currentNode == null)
                {
                    runningComponentLinkList.AddLast(component);
                    break;
                }

                if (currentNode.Value.Priority > component.Priority)
                {
                    runningComponentLinkList.AddBefore(currentNode, component);
                    break;
                }

                if (currentNode == runningComponentLinkList.Last)
                {
                    runningComponentLinkList.AddLast(component);
                    break;
                }

                currentNode = currentNode.Next;
            }

            componentDic.Add(component.ID, component);

            component.SetEntityLogic(this);
            component.OnComponentAttach();
            componentListChanged = true;
        }

        public void RemoveComponent(EntityComponentBase component)
        {
            RemoveComponent(component.ID);
        }

        public void RemoveComponent(int componentID)
        {
            if (!Available || !ComponentContentInit)
            {
                return;
            }

            if (componentDic.TryGetValue(componentID, out var component))
            {
                component.OnComponentDetach();
                componentDic.Remove(componentID);
            }
        }

        private void TryReleaseReferenceComponent(EntityComponentBase component)
        {
            if (component is IReference referenceComponent)
            {
                // Log.Info($"reference component release {referenceComponent.GetType().FullName}");
                ReferencePool.Release(referenceComponent);
            }
        }

        public bool ContainComponent(int componentID)
        {
            if (!ComponentContentInit)
            {
                return false;
            }

            return componentDic.TryGetValue(componentID, out var component) && component.Available;
        }

        public T GetComponent<T>(int componentId) where T : EntityComponentBase
        {
            var component = GetComponent(componentId);
            return component as T;
        }

        public EntityComponentBase GetComponent(int componentID)
        {
            if (!ComponentContentInit)
            {
                return null;
            }

            if (componentDic.TryGetValue(componentID, out var component) && component.Available)
            {
                return component;
            }

            return null;
        }

    #if UNITY_EDITOR
        private void RefreshComponentInfos()
        {
            sortedComponentInfos.Clear();

            foreach (var component in runningComponentLinkList)
            {
                if (component.Available)
                {
                    sortedComponentInfos.Add(new ComponentInfo(component,
                        needRemoveComponent =>
                        {
                            RemoveComponent(needRemoveComponent);
                            RefreshComponentInfos();
                        }));
                }
            }

            //更新搜索结果
            OnSearchingTextChanged();
        }
    #endif

        private int ComponentCompare(EntityComponentBase x, EntityComponentBase y)
        {
            return y.Priority - x.Priority;
        }

        private EventPool<GameEventArgs> componentEventPool =
            new EventPool<GameEventArgs>(EventPoolMode.AllowMultiHandler | EventPoolMode.AllowNoHandler);

        public void FireComponentInnerEvent(Object sender, GameEventArgs e)
        {
            componentEventPool.FireNow(sender, e);
        }

        public void SubscribeEntityInnerEvent(int eventId, EventHandler<GameEventArgs> e)
        {
            componentEventPool.Subscribe(eventId, e);
        }

        public void UnSubscribeEntityInnerEvent(int eventId, EventHandler<GameEventArgs> e)
        {
            componentEventPool.Unsubscribe(eventId, e);
        }

        /// <summary>
        /// 除了某些component,删除其余component
        /// </summary>
        /// <param name="exceptComponentIds"></param>
        public void RemoveAllComponentsExcept(HashSet<int> exceptComponentIds)
        {
            foreach (var component in runningComponentLinkList)
            {
                if (component.Available && !exceptComponentIds.Contains(component.ID))
                {
                    RemoveComponent(component);
                }
            }
        }

        public void RemoveAllComponents()
        {
            if (ComponentContentInit)
            {
                componentDic.Clear();
                foreach (var component in runningComponentLinkList)
                {
                    if (component.Available)
                    {
                        component.OnComponentDetach();
                        component.SetEntityLogic(null);
                    }

                    TryReleaseReferenceComponent(component);
                }

                runningComponentLinkList.Clear();

                componentEventPool?.Clear();

            #if UNITY_EDITOR
                sortedComponentInfos.Clear();
            #endif
            }

            if (lateUpdateEnabled)
            {
                Framework.RemoveLateUpdateEntity(this);
                lateUpdateEnabled = false;
            }

            if (fixedUpdateEnabled)
            {
                Framework.RemoveFixedUpdateEntity(this);
                fixedUpdateEnabled = false;
            }
        }

        public NativeArray<T> GetNativeArray<T>(int count) where T : struct
        {
            nativeArrayPool ??= new EntityNativeArrayPool();

            return nativeArrayPool.Get<T>(count);
        }

        public void ReleaseNativeArray<T>(NativeArray<T> nativeArray) where T : struct
        {
            nativeArrayPool?.Release(nativeArray);
        }

        public void ReleaseAllNativeArray()
        {
            nativeArrayPool?.ReleaseAll();
        }

    #if UNITY_EDITOR
        public void InvokeOnShow()
        {
            OnShow(null);
        }

        public void InvokeOnHide()
        {
            OnHide(true, null);
        }

        public void InvokeOnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected virtual void OnGUI()
        {
            if (Application.isPlaying && runningComponentLinkList != null)
            {
                foreach (var component in runningComponentLinkList)
                {
                    if (component.Available)
                    {
                        component.OnGUI();
                    }
                }
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (Application.isPlaying && runningComponentLinkList != null)
            {
                foreach (var component in runningComponentLinkList)
                {
                    if (component.Available)
                    {
                        component.OnDrawGizmos();
                    }
                }
            }
        }
    #endif
    }
}
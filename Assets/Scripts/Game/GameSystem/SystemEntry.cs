using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.GameSystem
{
    public class SystemEntry : MonoBehaviour
    {
        private static SystemEntry instance;

        private Dictionary<int, SystemBase> systemDic = new Dictionary<int, SystemBase>();
        private LinkedList<SystemBase> systemLinkedList = new LinkedList<SystemBase>();

    #if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("当前系统")]
        private List<string> currentSystemNames = new List<string>();
    #endif

        private void Awake()
        {
            instance = this;
        }

        private void OnDisable()
        {
            //该类应该始终存在，所以只可能是退出游戏或者editor停止播放时触发。
            //如果是退出游戏，那么操作系统会负责完成所有回收工作。
            //如果是editor停止播放，那么一般也不需要调用system的OnDisable()，因为unity会完成回收工作
            //但是某些操作，unity无法完成，例如NativeArray的释放，线程的停止释放，或者是游戏运行过程中修改了工程配置，完成运行时，需要将配置改回来。
            Time.timeScale = 1f;
        #if UNITY_EDITOR
            var currentNode = systemLinkedList.First;
            while (currentNode != null)
            {
                if (!currentNode.Value.Available)
                {
                    var nextNode = currentNode.Next;
                    systemLinkedList.Remove(currentNode);
                    currentNode = nextNode;
                }
                else
                {
                    //这里不能调用System的OnDisable方法。因为该方法中可能会添加/删除其他system或者创建/销毁某些GameObject，
                    //这种情况可能会造成unity报错。所以使用OnEditorStopPlay方法，该方法的目的只能是释放那些必须手动释放或者重置的内容。
                    currentNode.Value.OnEditorStopPlay();
                    currentNode = currentNode.Next;
                }
            }

            return;
        #endif

            throw new Exception("非unity editor模式下，SystemEntry实例不应该被disable或者销毁");

            // #if UNITY_EDITOR
            //     var universalRenderPipelineAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            //     if (universalRenderPipelineAsset != null)
            //     {
            //         universalRenderPipelineAsset.renderScale = 1;
            //     }
            // #endif
            //
            //     if (Framework.IsApplicationQuitting)
            //     {
            //         return;
            //     }
            //
            //     foreach (var keyValuePair in systemDic)
            //     {
            //         var system = keyValuePair.Value;
            //         if (system.Available)
            //         {
            //             system.OnDisable();
            //         }
            //     }
            //
            //     systemDic.Clear();
            //     systemLinkedList.Clear();
        }

        private void Update()
        {
            var currentNode = systemLinkedList.First;
            while (currentNode != null)
            {
                if (!currentNode.Value.Available)
                {
                    var nextNode = currentNode.Next;
                    systemLinkedList.Remove(currentNode);
                    currentNode = nextNode;
                }
                else
                {
                    currentNode.Value.OnUpdate(Time.deltaTime, Time.unscaledDeltaTime);
                    currentNode = currentNode.Next;
                }
            }
        }

        public static void AddSystem(SystemBase system)
        {
            if (ContainSystem(system.ID))
            {
                throw new Exception($"重复添加System. type {system.GetType().Name}");
            }

            instance.systemDic.Add(system.ID, system);
            var systemLinkedList = instance.systemLinkedList;
            var currentNode = systemLinkedList.First;
            if (currentNode == null)
            {
                systemLinkedList.AddFirst(system);
            }
            else
            {
                var systemInserted = false;
                while (currentNode != null)
                {
                    if (currentNode.Value.Priority > system.Priority)
                    {
                        systemInserted = true;
                        systemLinkedList.AddBefore(currentNode, system);
                        break;
                    }

                    currentNode = currentNode.Next;
                }

                if (!systemInserted)
                {
                    systemLinkedList.AddLast(system);
                }
            }

            system.OnEnable();
            RefreshSystemNames();
        }

        public static void RemoveSystem(int systemID)
        {
            if (!ContainSystem(systemID))
            {
                return;
            }

            var systemDic = instance.systemDic;
            var system = systemDic[systemID];
            system.OnDisable();
            systemDic.Remove(systemID);
            RefreshSystemNames();
        }

        public static void RemoveSystem(SystemBase system)
        {
            RemoveSystem(system.ID);
        }

        public static SystemBase GetSystem(int systemID)
        {
            if (instance == null)
            {
                return null;
            }

            if (!ContainSystem(systemID))
            {
                return null;
            }

            var systemDic = instance.systemDic;
            return systemDic[systemID];
        }

        public static bool ContainSystem(int systemID)
        {
            var systemDic = instance.systemDic;
            return systemDic.TryGetValue(systemID, out var system) && system.Available;
        }

        private static void RefreshSystemNames()
        {
        #if UNITY_EDITOR
            var currentSystemNames = instance.currentSystemNames;
            currentSystemNames.Clear();

            var systemLinkedList = instance.systemLinkedList;
            var currentNode = systemLinkedList.First;
            while (currentNode != null)
            {
                if (currentNode.Value.Available)
                {
                    currentSystemNames.Add(currentNode.Value.GetType().FullName);
                }

                currentNode = currentNode.Next;
            }
        #endif
        }

    #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            var current = systemLinkedList.First;
            while (current != null)
            {
                if (current.Value.Available)
                {
                    current.Value.OnDrawGizmos();
                }

                current = current.Next;
            }
        }
    #endif
    }
}
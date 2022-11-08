using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Common
{
    /// <summary>
    /// 调用update,lateupdate,fixedupdate方法
    /// </summary>
    public class CommonUnityUpdateFunctionCaller : MonoBehaviour
    {
        private static CommonUnityUpdateFunctionCaller current;
        public static CommonUnityUpdateFunctionCaller Current => current;

        private class FunctionWrapper
        {
            public bool isAvailable;
            public Action<float, float> func;
        }

        private List<FunctionWrapper> updateFunctionWrappers = new List<FunctionWrapper>();
        private List<FunctionWrapper> fixedUpdateFunctionWrappers = new List<FunctionWrapper>();
        private List<FunctionWrapper> lateUpdateFunctionWrappers = new List<FunctionWrapper>();
        private Dictionary<Action<float, float>, FunctionWrapper> funcToWrapperMap =
            new Dictionary<Action<float, float>, FunctionWrapper>();

        private void Awake()
        {
            current = this;
        }

        private void OnDestroy()
        {
            if (!Framework.IsApplicationQuitting)
            {
                throw new Exception($"{nameof(CommonUnityUpdateFunctionCaller)}应该始终存在");
            }
        }

        public static void RegisterUpdateCall(Action<float, float> func)
        {
            var wrapper = new FunctionWrapper
            {
                func = func,
                isAvailable = true
            };
            current.updateFunctionWrappers.Add(wrapper);
            current.funcToWrapperMap.Add(func, wrapper);
        }

        public static void UnRegisterUpdateCall(Action<float, float> func)
        {
            if (current.funcToWrapperMap.TryGetValue(func, out var wrapper))
            {
                wrapper.isAvailable = false;
            }
        }

        public static void RegisterFixedUpdateCall(Action<float, float> func)
        {
            var wrapper = new FunctionWrapper
            {
                func = func,
                isAvailable = true
            };
            current.fixedUpdateFunctionWrappers.Add(wrapper);
            current.funcToWrapperMap.Add(func, wrapper);
        }

        public static void UnRegisterFixedUpdateCall(Action<float, float> func)
        {
            //目前的实现，注销都是一样的逻辑
            UnRegisterUpdateCall(func);
        }

        public static void RegisterLateUpdateCall(Action<float, float> func)
        {
            var wrapper = new FunctionWrapper
            {
                func = func,
                isAvailable = true
            };
            current.lateUpdateFunctionWrappers.Add(wrapper);
            current.funcToWrapperMap.Add(func, wrapper);
        }

        public static void UnRegisterLateUpdateCall(Action<float, float> func)
        {
            //目前的实现，注销都是一样的逻辑
            UnRegisterUpdateCall(func);
        }

        private void Update()
        {
            var elapseSeconds = Time.deltaTime;
            var realElapseSeconds = Time.unscaledDeltaTime;
            for (var i = updateFunctionWrappers.Count - 1; i >= 0; i--)
            {
                var updateFunctionWrapper = updateFunctionWrappers[i];
                if (updateFunctionWrapper.isAvailable)
                {
                    updateFunctionWrapper.func.Invoke(elapseSeconds, realElapseSeconds);
                }
                else
                {
                    updateFunctionWrappers.RemoveAt(i);
                }
            }
        }

        private void FixedUpdate()
        {
            var elapseSeconds = Time.fixedDeltaTime;
            var realElapseSeconds = Time.fixedUnscaledDeltaTime;
            for (var i = fixedUpdateFunctionWrappers.Count - 1; i >= 0; i--)
            {
                var fixedUpdateFunctionWrapper = fixedUpdateFunctionWrappers[i];
                if (fixedUpdateFunctionWrapper.isAvailable)
                {
                    fixedUpdateFunctionWrapper.func.Invoke(elapseSeconds, realElapseSeconds);
                }
                else
                {
                    fixedUpdateFunctionWrappers.RemoveAt(i);
                }
            }
        }

        private void LateUpdate()
        {
            var elapseSeconds = Time.deltaTime;
            var realElapseSeconds = Time.unscaledDeltaTime;
            for (var i = lateUpdateFunctionWrappers.Count - 1; i >= 0; i--)
            {
                var lateUpdateFunctionWrapper = lateUpdateFunctionWrappers[i];
                if (lateUpdateFunctionWrapper.isAvailable)
                {
                    lateUpdateFunctionWrapper.func.Invoke(elapseSeconds, realElapseSeconds);
                }
                else
                {
                    lateUpdateFunctionWrappers.RemoveAt(i);
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameSystem
{
    public class CoroutineSystem : SystemBase
    {
        public static CoroutineSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as CoroutineSystem;
        }

        private static int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private Dictionary<Object, IEnumerator> bindCoroutines = new Dictionary<Object, IEnumerator>();
        private List<Object> needRemoveTargets = new List<Object>();

        private GameObject coroutineRoot;
        private CoroutineHelper coroutineHelper;

        public class CoroutineHelper : MonoBehaviour
        {
        }

        internal override void OnEnable()
        {
            base.OnEnable();
            coroutineRoot = new GameObject("CoroutineRoot");
            coroutineHelper = coroutineRoot.AddComponent<CoroutineHelper>();
        }

        internal override void OnDisable()
        {
            base.OnDisable();
            Object.Destroy(coroutineRoot);
            coroutineRoot = null;
            coroutineHelper = null;
        }

        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (bindCoroutines.Count > 0)
            {
                needRemoveTargets.Clear();
                foreach (var keyValuePair in bindCoroutines)
                {
                    var iEnumerator = keyValuePair.Value;
                    var target = keyValuePair.Key;
                    if (iEnumerator.Current == null)
                    {
                        needRemoveTargets.Add(target);
                    }
                }
                
                foreach (var needRemoveTarget in needRemoveTargets)
                {
                    bindCoroutines.Remove(needRemoveTarget);
                }
            }
        }

        public IEnumerator StartCoroutine(IEnumerator iEnumerator)
        {
            if (iEnumerator == null || coroutineHelper == null)
            {
                return null;
            }

            coroutineHelper.StartCoroutine(iEnumerator);
            return iEnumerator;
        }

        public void StopCoroutine(IEnumerator iEnumerator)
        {
            if (iEnumerator != null &&  coroutineHelper != null)
            {
                coroutineHelper.StopCoroutine(iEnumerator);
            }
        }

        public void BindTarget(IEnumerator iEnumerator, Object target)
        {
            if (iEnumerator == null || target == null)
            {
                return;
            }

            if (bindCoroutines.ContainsKey(target))
            {
                Log.Error($"重复绑定了target {target}");
                return;
            }

            bindCoroutines.Add(target, iEnumerator);
        }

        public void Kill(Object target)
        {
            if (target.Equals(null))
            {
                return;
            }

            if (bindCoroutines.TryGetValue(target, out var iEnumerator))
            {
                StopCoroutine(iEnumerator);
                bindCoroutines.Remove(target);
            }
        }
    }
}
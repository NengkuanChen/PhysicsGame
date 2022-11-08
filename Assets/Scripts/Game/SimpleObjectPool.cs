using System;
using System.Collections.Generic;

namespace Game
{
    public class SimpleObjectPool<T> : IDisposable
    {
        private List<T> freeObjectList = new List<T>();
        private HashSet<T> obtainedObjectSet = new HashSet<T>();

        private Func<T> onInstantiate;
        private Action<T> onGet;
        private Action<T> onRelease;
        private Action<T> onDispose;

        public SimpleObjectPool(Func<T> onInstantiate,
                                Action<T> onGet,
                                Action<T> onRelease = null,
                                Action<T> onDispose = null)
        {
            this.onInstantiate = onInstantiate;
            this.onGet = onGet;
            this.onRelease = onRelease;
            this.onDispose = onDispose;
        }

        public T Spawn()
        {
            if (freeObjectList.Count == 0)
            {
                var instance = onInstantiate();
                onGet?.Invoke(instance);
                obtainedObjectSet.Add(instance);
                return instance;
            }

            var o = freeObjectList[freeObjectList.Count - 1];
            freeObjectList.RemoveAt(freeObjectList.Count - 1);
            obtainedObjectSet.Add(o);
            onGet?.Invoke(o);
            return o;
        }

        public void Release(T o)
        {
            if (o == null)
            {
                return;
            }
        #if UNITY_EDITOR
            if (!obtainedObjectSet.Contains(o))
            {
                throw new ArgumentException($"object release multiple times: {o.GetType().Name}");
            }
        #endif
            onRelease?.Invoke(o);
            obtainedObjectSet.Remove(o);
            freeObjectList.Add(o);
        }

        public void ReleaseAllObtainedObjects()
        {
            foreach (var o in obtainedObjectSet)
            {
                onRelease?.Invoke(o);
                freeObjectList.Add(o);
            }

            obtainedObjectSet.Clear();
        }

        public void Dispose()
        {
            if (onDispose != null)
            {
                foreach (var o in freeObjectList)
                {
                    onDispose(o);
                }

                foreach (var o in obtainedObjectSet)
                {
                    onDispose(o);
                }
            }

            freeObjectList.Clear();
            obtainedObjectSet.Clear();
        }
    }
}
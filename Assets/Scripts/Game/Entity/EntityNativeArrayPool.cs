using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Game.Entity
{
    public class EntityNativeArrayPool
    {
        private abstract class Pool
        {
            public abstract void Release();
        }

        private class GenericPool<T> : Pool where T : struct
        {
            private HashSet<NativeArray<T>> arrays = new HashSet<NativeArray<T>>();
            public HashSet<NativeArray<T>> Arrays => arrays;

            public override void Release()
            {
                foreach (var nativeArray in arrays)
                {
                    if (nativeArray.IsCreated)
                    {
                        // Log.Info("release native array");
                        nativeArray.Dispose();
                    }
                }

                arrays.Clear();
            }
        }

        private Dictionary<Type, Pool> nativeArrayPool = new Dictionary<Type, Pool>();

        public NativeArray<T> Get<T>(int count) where T : struct
        {
            var t = typeof(T);
            if (!nativeArrayPool.TryGetValue(t, out var pool))
            {
                pool = new GenericPool<T>();
                nativeArrayPool.Add(t, pool);
            }

            var genericPool = pool as GenericPool<T>;
            var arrays = genericPool.Arrays;
            var newNativeArray = new NativeArray<T>(count, Allocator.Persistent);
            arrays.Add(newNativeArray);
            return newNativeArray;
        }

        public void Release<T>(NativeArray<T> nativeArray) where T : struct
        {
            if (!nativeArray.IsCreated)
            {
                return;
            }

            var t = typeof(T);
            if (!nativeArrayPool.TryGetValue(t, out var pool))
            {
                return;
            }

            var genericPool = pool as GenericPool<T>;
            var arrays = genericPool.Arrays;
            arrays.Remove(nativeArray);
            nativeArray.Dispose();
            // Log.Info($"release native array. type {t.Name}");
        }

        public void ReleaseAll()
        {
            foreach (var keyValuePair in nativeArrayPool)
            {
                var pool = keyValuePair.Value;
                pool.Release();
            }
        }
    }
}
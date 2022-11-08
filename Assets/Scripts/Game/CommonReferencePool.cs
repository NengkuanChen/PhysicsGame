using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public static class CommonReferencePool
    {
        private static Dictionary<Type, List<object>> pools = new Dictionary<Type, List<object>>();

        public static T Get<T>() where T : class, new()
        {
            var type = typeof(T);
            if (pools.TryGetValue(type, out var pool))
            {
                if (pool.Count == 0)
                {
                    return new T();
                }

                var result = pool.Last();
                pool.RemoveAt(pool.Count - 1);
                return result as T;
            }
            else
            {
                pools.Add(type, new List<object>());
                return new T();
            }
        }

        public static void Release(object o)
        {
            var type = o.GetType();
            if (pools.TryGetValue(type,out var pool))
            {
                pool.Add(o);
            }
            else
            {
                throw new Exception($"release object {type.Name} is not get from pool");
            }
        }
    }
}
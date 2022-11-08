using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Entity;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using RangeInt = Game.Number.RangeInt;

namespace Game.Utility
{
    public static class ExtensionUtility
    {
        public static double ConvertToUnixTimestamp(this DateTime dt)
        {
            var from = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var span = dt.ToUniversalTime() - from;
            return Math.Floor(span.TotalSeconds);
        }

        public static void Play(this ParticleSystem[] fxs)
        {
            if (fxs == null || fxs.Length == 0)
            {
                return;
            }

            foreach (var fx in fxs)
            {
                fx.Play(true);
            }
        }

        public static void Stop(this ParticleSystem[] fxs)
        {
            if (fxs == null || fxs.Length == 0)
            {
                return;
            }

            foreach (var fx in fxs)
            {
                fx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        public static void StopAndClear(this ParticleSystem[] fxs)
        {
            if (fxs == null || fxs.Length == 0)
            {
                return;
            }

            foreach (var fx in fxs)
            {
                fx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        public static void DisablePlayOnAwake(this ParticleSystem[] fxs)
        {
            if (fxs == null || fxs.Length == 0)
            {
                return;
            }

            foreach (var fx in fxs)
            {
                var mainModule = fx.main;
                mainModule.playOnAwake = false;
            }
        }

        public static void SetAlpha(this Graphic graphic, float a)
        {
            if (graphic == null)
            {
                return;
            }

            var color = graphic.color;
            color.a = a;
            graphic.color = color;
        }

        public static void SetActive(this IEnumerable<GameObject> gameObjects, bool isActive)
        {
            if (gameObjects == null)
            {
                return;
            }

            foreach (var gameObject in gameObjects)
            {
                if (gameObject == null)
                {
                    continue;
                }

                gameObject.SetActive(isActive);
            }
        }

        public static void SetEnable(this IList<Collider> colliders, bool isEnable)
        {
            if (colliders == null)
            {
                return;
            }

            foreach (var collider in colliders)
            {
                collider.enabled = isEnable;
            }
        }

        public static Vector3 GetAverageCenterPosition(this IList<Collider> colliders)
        {
            if (colliders == null)
            {
                throw new ArgumentNullException(nameof(colliders));
            }

            var result = Vector3.zero;
            foreach (var collider in colliders)
            {
                result += collider.bounds.center;
            }

            result /= colliders.Count;
            return result;
        }

        public static int Random(this RangeInt rangeInt)
        {
            return UnityEngine.Random.Range(rangeInt.min, rangeInt.max + 1);
        }

        public static string PathInScene(this GameObject gameObject)
        {
            if (gameObject == null)
            {
                return "null game object";
            }

            var transform = gameObject.transform;
            var pathList = new List<string>();
            while (transform != null)
            {
                pathList.Add(transform.name);
                transform = transform.parent;
            }

            var sb = new StringBuilder();
            for (var i = pathList.Count - 1; i >= 0; i--)
            {
                sb.Append(pathList[i]);
                sb.Append("/");
            }

            return sb.ToString();
        }

        public static void ResetTransformLocal(this Transform transform,
                                               bool ignorePosition = false,
                                               bool ignoreRotation = false,
                                               bool ignoreScale = false)
        {
            if (transform == null)
            {
                return;
            }

            if (!ignorePosition)
            {
                transform.localPosition = Vector3.zero;
            }

            if (!ignoreRotation)
            {
                transform.localRotation = Quaternion.identity;
            }

            if (!ignoreScale)
            {
                transform.localScale = Vector3.one;
            }
        }

        public static void SetColor(this RenderTexture rt, Color c)
        {
            if (rt == null)
            {
                return;
            }

            var oldRt = RenderTexture.active;

            RenderTexture.active = rt;
            GL.Clear(true, true, c);

            RenderTexture.active = oldRt;
        }

        public static void Hide(this GameEntityLogic entityLogic)
        {
            if (entityLogic == null)
            {
                return;
            }

            EntityUtility.HideEntity(entityLogic);
        }

        public static T RandomElement<T>(this IList<T> l)
        {
            if (l == null || l.Count == 0)
            {
                return default;
            }

            return l[UnityEngine.Random.Range(0, l.Count)];
        }

        public static void Shuffle<T>(this IList<T> list, int shuffleCount = -1)
        {
            if (list == null)
            {
                return;
            }

            if (shuffleCount >= 0)
            {
                shuffleCount = Mathf.Min(list.Count, shuffleCount);
            }
            else
            {
                shuffleCount = list.Count;
            }

            for (var i = 0; i < shuffleCount - 1; i++)
            {
                var randomIndex = UnityEngine.Random.Range(i + 1, list.Count);
                var temp = list[randomIndex];
                list[randomIndex] = list[i];
                list[i] = temp;
            }
        }

        public static void AlignTransforms(this Transform from, Transform target)
        {
            if (from == null || target == null)
            {
                return;
            }

            from.position = target.position;
            from.rotation = target.rotation;
        }

        public static T FindComponentThroughParent<T>(this Transform startTransform) where T : Component
        {
            if (startTransform == null)
            {
                return null;
            }

            var currentTransform = startTransform.parent;
            while (currentTransform != null)
            {
                var result = currentTransform.GetComponent<T>();
                if (result != null)
                {
                    return result;
                }

                currentTransform = currentTransform.parent;
            }

            return null;
        }

        public static string[] GetHierarchyNames(this Transform target, Transform stopNode = null)
        {
            var result = new List<string>();
            while (target != null && target != stopNode)
            {
                result.Add(target.gameObject.name);
                target = target.parent;
            }

            result.Reverse();
            return result.ToArray();
        }

        public static Transform FindChildByHierarchyNames(this Transform parent, string[] hierarchyNames)
        {
            if (parent == null || hierarchyNames == null || hierarchyNames.Length == 0)
            {
                return null;
            }

            var result = parent;
            foreach (var hierarchyName in hierarchyNames)
            {
                result = result.Find(hierarchyName);
                if (result == null)
                {
                    return null;
                }
            }

            return result;
        }

        public static void CreateHierarchy(this Transform root, string hierarchyPath)
        {
            CreateHierarchy(root, hierarchyPath.Split('/'));
        }

        public static void CreateHierarchy(this Transform root, string[] hierarchyNames)
        {
            foreach (var s in hierarchyNames)
            {
                var t = new GameObject(s).transform;
                t.SetParent(root);
                t.ResetTransformLocal();
                root = t;
            }
        }

        public static Vector3 ChannelMultiply(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 ChannelDivide(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector4 ToPositionVector4(this Vector3 p)
        {
            Vector4 result = p;
            result.w = 1f;
            return result;
        }

        public static void TryUnSubscribe(this EventComponent eventComponent,
                                          int eventId,
                                          EventHandler<GameEventArgs> handler)
        {
            if (eventComponent.Check(eventId, handler))
            {
                eventComponent.Unsubscribe(eventId, handler);
            }
        }
    }
}
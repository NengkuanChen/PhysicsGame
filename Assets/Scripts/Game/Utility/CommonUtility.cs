using System;
using System.Collections;
using System.Collections.Generic;
using Game.Entity;
using Game.GameSystem;
using Game.Procedure;
using GameFramework;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEngine.Assertions;
using Random = System.Random;

namespace Game.Utility
{
    public static class CommonUtility
    {
        public static Color WhiteClearColor = new Color(1f, 1f, 1f, 0f);
        public static bool RandomBoolean => UnityEngine.Random.Range(0f, 1f) > .5f;

        public static Collider[] CacheOneColliderArray = new Collider[1];

        public static bool IsWidthScreen
        {
            get
            {
                const float widthRatio = 1080f / 1920f;
                var currentRatio = (float) Screen.width / Screen.height;
                return currentRatio > widthRatio;
            }
        }

        public static float WidthScreenWidthScale
        {
            get
            {
                return IsWidthScreen switch
                {
                    true => Screen.height * (1080f / 1920f) / Screen.width,
                    _    => 1f
                };
            }
        }

        /// <summary>
        /// 设置相机渲染范围,宽屏两边会加黑边
        /// </summary>
        /// <param name="c"></param>
        public static void CameraRectAdapte(Camera c)
        {
            if (c == null)
            {
                return;
            }

            //宽屏两边加黑边
            if (IsWidthScreen)
            {
                var width = Screen.height * (1080f / 1920f) / Screen.width;
                c.rect = new Rect((1f - width) * .5f, 0f, width, 1f);
            }
        }

        public static void RestoreCameraRect(Camera c)
        {
            if (c == null)
            {
                return;
            }

            c.rect = new Rect(0f, 0f, 1f, 1f);
        }

        public static bool ScreenHaveNotch
        {
            get
            {
                var safeArea = Screen.safeArea;
                return !Mathf.Approximately(safeArea.width, Screen.width) ||
                       !Mathf.Approximately(safeArea.height, Screen.height);
            }
        }

        public static double UtcNow => DateTime.UtcNow.ConvertToUnixTimestamp();

        public static void Clear(this RenderTexture rt)
        {
            if (rt == null)
            {
                return;
            }

            var oldRt = RenderTexture.active;

            RenderTexture.active = rt;
            GL.Clear(true, true, Color.clear);

            RenderTexture.active = oldRt;
        }

        public static string GetPlaceSuffix(int place)
        {
            if (place == 1)
            {
                return "st";
            }

            if (place == 2)
            {
                return "nd";
            }

            if (place == 3)
            {
                return "rd";
            }

            return "th";
        }

        public static T CreateComponentFromReferencePool<T>() where T : class, IReference, new()
        {
            var result = ReferencePool.Acquire<T>();
            if (result is EntityComponentBase component)
            {
                component.IsCreatedFromReferencePool = true;
            }

            return result;
        }

        public static int CombineHashCodes(int h1, int h2)
        {
            // this is where the magic happens
            return (((h1 << 5) + h1) ^ h2);
        }

        public static void ShuffleArray<T>(IList<T> array, int startShuffleIndex = 0)
        {
            if (array.Count <= 1)
            {
                return;
            }

            if (startShuffleIndex >= array.Count)
            {
                return;
            }

            for (var i = startShuffleIndex; i < array.Count; i++)
            {
                var randomIndex = UnityEngine.Random.Range(i, array.Count);
                var temp = array[i];
                array[i] = array[randomIndex];
                array[randomIndex] = temp;
            }
        }

        public static bool WorldPointToLocalPointInRectangle(RectTransform rectTransform,
                                                             Vector3 worldPosition,
                                                             out Vector2 localPoint)
        {
            if (GameMainCamera.Current == null)
            {
                throw new Exception($"场景中没有{nameof(GameMainCamera)}");
            }

            var positionInViewPort = GameMainCamera.Current.Camera.WorldToViewportPoint(worldPosition);
            var screenPoint = UICamera.Current.Camera.ViewportToScreenPoint(positionInViewPort);
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
                screenPoint,
                UICamera.Current.Camera,
                out localPoint);
        }

    #region CoroutineHelper

        public static IEnumerator StartCoroutine(this IEnumerator iEnumerator)
        {
            var coroutineSystem = CoroutineSystem.Get();
            return coroutineSystem?.StartCoroutine(iEnumerator);
        }

        public static void StopCoroutine(this IEnumerator iEnumerator)
        {
            var coroutineSystem = CoroutineSystem.Get();
            coroutineSystem?.StopCoroutine(iEnumerator);
        }

        public static void BindTarget(this IEnumerator iEnumerator, Object target)
        {
            var coroutineSystem = CoroutineSystem.Get();
            coroutineSystem?.BindTarget(iEnumerator, target);
        }

        public static void KillCoroutine(this Object target)
        {
            var coroutineSystem = CoroutineSystem.Get();
            coroutineSystem?.Kill(target);
        }

    #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameFramework.Resource;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Utility
{
    public static class ResourceUtility
    {
        private static HashSet<int> loadingResourceIdSet = new HashSet<int>();
        private static int nextResourceId;
        public static int NextResourceId
        {
            get
            {
                var id = nextResourceId++;
                loadingResourceIdSet.Add(id);
                return id;
            }
        }

        /// <summary>
        /// 载入资源
        /// 路径从Assets/Game开始
        /// </summary>
        /// <param name="relativeAssetPath"></param>
        /// <param name="onLoadComplete"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int LoadAssetWithRelativePath<T>(string relativeAssetPath, Action<T> onLoadComplete)
            where T : Object
        {
            var absolutePath = AssetPathUtility.GetCommonAssetPath(relativeAssetPath);
            return LoadAssetWithAbsolutePath(absolutePath, onLoadComplete);
        }

        /// <summary>
        /// 载入资源
        /// 路径从头开始,也就是Assets/...
        /// </summary>
        /// <param name="absoluteAssetPath"></param>
        /// <param name="onLoadComplete"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int LoadAssetWithAbsolutePath<T>(string absoluteAssetPath, Action<T> onLoadComplete)
            where T : Object
        {
            if (onLoadComplete == null)
            {
                return -1;
            }

            var resourceId = NextResourceId;
            Framework.ResourceComponent.LoadAsset(absoluteAssetPath,
                typeof(T),
                new LoadAssetCallbacks((assetName, asset, duration, userData) =>
                    {
                        if (loadingResourceIdSet.Contains(resourceId))
                        {
                            loadingResourceIdSet.Remove(resourceId);
                            onLoadComplete(asset as T);
                        }
                        else
                        {
                            UnLoadAsset(asset as Object);
                        }
                    },
                    (assetName, status, message, userData) => { Log.Error($"无法载入资源{assetName},{message}"); }));
            return resourceId;
        }

        public static async UniTask<T> LoadAssetAsync<T>(string assetPath, bool isRelativePath = true) where T : Object
        {
            T loadedAsset = null;
            if (isRelativePath)
            {
                LoadAssetWithRelativePath<T>(assetPath, asset => loadedAsset = asset);
            }
            else
            {
                LoadAssetWithAbsolutePath<T>(assetPath, asset => loadedAsset = asset);
            }

            while (loadedAsset == null)
            {
                await Task.Yield();
            }

            return loadedAsset;
        }

        public static async UniTask<T> LoadAssetAsync<T>(string assetPath, CancellationToken cs) where T : Object
        {
            T loadedAsset = null;
            LoadAssetWithRelativePath<T>(assetPath, asset => loadedAsset = asset);
            while (loadedAsset == null)
            {
                await Task.Yield();
            }

            if (cs.IsCancellationRequested)
            {
                UnLoadAsset(loadedAsset);
                return null;
            }

            return loadedAsset;
        }

        private static readonly Type SpriteType = typeof(Sprite);

        /// <summary>
        /// 载入sprite
        /// </summary>
        /// <param name="spritePath">相对路径,从Assets/Game/UI/Sprites路径开始</param>
        /// <param name="onLoadComplete"></param>
        public static int LoadSprite(string spritePath, Action<Sprite> onLoadComplete)
        {
            var resourceId = NextResourceId;
            Framework.ResourceComponent.LoadAsset(AssetPathUtility.GetSpritePath(spritePath),
                SpriteType,
                new LoadAssetCallbacks((assetName, asset, duration, userData) =>
                    {
                        if (loadingResourceIdSet.Contains(resourceId))
                        {
                            loadingResourceIdSet.Remove(resourceId);
                            onLoadComplete(asset as Sprite);
                        }
                        else
                        {
                            UnLoadAsset(asset as Object);
                        }
                    },
                    (assetName, status, message, userData) =>
                    {
                        Log.Error($"load asset error. asset: {assetName}, {message}");
                    }));
            return resourceId;
        }

        public static void UnLoadAsset(Object asset)
        {
            if (asset == null)
            {
                return;
            }

            var resourceComponent = Framework.ResourceComponent;
            if (resourceComponent == null)
            {
                return;
            }

            resourceComponent.UnloadAsset(asset);
        }

        public static void TryStopLoadingResource(int resourceId)
        {
            if (loadingResourceIdSet.Contains(resourceId))
            {
                loadingResourceIdSet.Remove(resourceId);
            }
        }
    }
}
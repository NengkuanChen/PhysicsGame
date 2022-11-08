using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Utility;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Game.GameSystem
{
    public class SceneSystem : SystemBase
    {
        public static SceneSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as SceneSystem;
        }

        private static int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        public class LoadSceneCallbacks : IReference
        {
            public Action onLoadComplete;
            public Action onLoadFailure;
            public Action<float> onLoadUpdate;

            public void Clear()
            {
                onLoadComplete = null;
                onLoadFailure = null;
                onLoadUpdate = null;
            }
        }

        public class UnloadSceneCallbacks : IReference
        {
            public Action onUnloadSuccess;
            public Action onUnloadFailed;

            public void Clear()
            {
                onUnloadSuccess = null;
                onUnloadFailed = null;
            }
        }

        public SceneSystem()
        {
            var eventComponent = Framework.EventComponent;
            eventComponent.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneComplete);
            eventComponent.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            eventComponent.Subscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            eventComponent.Subscribe(UnloadSceneSuccessEventArgs.EventId, OnUnloadSceneSuccess);
        }

        internal override void OnDisable()
        {
            base.OnDisable();

            var eventComponent = Framework.EventComponent;
            eventComponent.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneComplete);
            eventComponent.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            eventComponent.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            eventComponent.Unsubscribe(UnloadSceneSuccessEventArgs.EventId, OnUnloadSceneSuccess);
        }

        private void OnLoadSceneComplete(object sender, GameEventArgs e)
        {
            if (!(e is LoadSceneSuccessEventArgs args))
            {
                return;
            }

            if (!(args.UserData is LoadSceneCallbacks callbacks))
            {
                return;
            }

            callbacks.onLoadComplete?.Invoke();
            ReferencePool.Release(callbacks);
        }

        private void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            if (!(e is LoadSceneSuccessEventArgs args))
            {
                return;
            }

            if (!(args.UserData is LoadSceneCallbacks callbacks))
            {
                return;
            }

            callbacks.onLoadFailure?.Invoke();
            ReferencePool.Release(callbacks);
        }

        private void OnLoadSceneUpdate(object sender, GameEventArgs e)
        {
            if (e is LoadSceneUpdateEventArgs args && args.UserData is LoadSceneCallbacks callbacks)
            {
                callbacks.onLoadUpdate?.Invoke(args.Progress);
            }
        }

        public void LoadScene(string sceneName, Action onLoadComplete, Action<float> onLoadUpdate)
        {
            var callbacks = ReferencePool.Acquire<LoadSceneCallbacks>();
            callbacks.onLoadComplete = onLoadComplete;
            callbacks.onLoadUpdate = onLoadUpdate;
        #if UNITY_EDITOR
            callbacks.onLoadFailure = () =>
            {
                Log.Info($"load scene {sceneName} failed");
            };
        #endif
            Framework.SceneComponent.LoadScene(AssetPathUtility.GetSceneAssetPath(sceneName), 0, callbacks);
        }

        public void UnloadScene(string sceneName, Action onComplete = null)
        {
            if (onComplete != null)
            {
                var callbacks = ReferencePool.Acquire<UnloadSceneCallbacks>();
                callbacks.onUnloadSuccess = onComplete;
                Framework.SceneComponent.UnloadScene(AssetPathUtility.GetSceneAssetPath(sceneName), callbacks);
            }
            else
            {
                Framework.SceneComponent.UnloadScene(AssetPathUtility.GetSceneAssetPath(sceneName));
            }
        }

        public async UniTask UnloadSceneAsync(string sceneName)
        {
            var unloadComplete = false;
            UnloadScene(sceneName, () => unloadComplete = true);
            while (!unloadComplete)
            {
                await Task.Yield();
            }
        }

        private void OnUnloadSceneSuccess(object sender, GameEventArgs e)
        {
            if (e is UnloadSceneSuccessEventArgs args && args.UserData is UnloadSceneCallbacks callbacks)
            {
                callbacks.onUnloadSuccess?.Invoke();
            }
        }
    }
}
using System;
using Cysharp.Threading.Tasks;

namespace Game.GameSystem
{
    public static class SceneUtility
    {
        public static void LoadScene(string sceneName, Action onComplete, Action<float> onLoadUpdate = null)
        {
            var sceneSystem = SceneSystem.Get();
            if (sceneSystem != null)
            {
                sceneSystem.LoadScene(sceneName, onComplete, onLoadUpdate);
            }
        }

        public static UniTask LoadSceneAsync(string sceneName, IProgress<float> progress = null)
        {
            var utc = new UniTaskCompletionSource();
            var sceneSystem = SceneSystem.Get();
            if (sceneSystem != null)
            {
                if (progress == null)
                {
                    sceneSystem.LoadScene(sceneName, () => utc.TrySetResult(), null);
                }
                else
                {
                    sceneSystem.LoadScene(sceneName, () => utc.TrySetResult(), progress.Report);
                }
            }

            return utc.Task;
        }

        public static void UnloadScene(string sceneName)
        {
            var sceneSystem = SceneSystem.Get();
            if (sceneSystem != null)
            {
                sceneSystem.UnloadScene(sceneName);
            }
        }

        public static async UniTask UnloadSceneAsync(string sceneName)
        {
            var sceneSystem = SceneSystem.Get();
            if (sceneSystem != null)
            {
                await sceneSystem.UnloadSceneAsync(sceneName);
            }
        }
    }
}

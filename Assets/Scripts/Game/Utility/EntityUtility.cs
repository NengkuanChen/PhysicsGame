using System;
using Cysharp.Threading.Tasks;
using Game.Entity;
using Game.GameSystem;

namespace Game.Utility
{
    public static class EntityUtility
    {
        /// <summary>
        /// 显示一个entity
        /// </summary>
        /// <param name="entityAssetPath">从Assets/Game/Entity目录开始的相对路径</param>
        /// <param name="groupName"><see cref="EntityGroupName"/></param>
        /// <param name="onShowComplete"></param>
        public static int ShowEntity(string entityAssetPath, string groupName, Action<GameEntityLogic> onShowComplete)
        {
            var entitySystem = EntitySystem.Get();
            if (entitySystem != null)
            {
                return entitySystem.ShowEntity(entityAssetPath, groupName, onShowComplete);
            }

            return -1;
        }

        /// <summary>
        /// 异步显示一个entity
        /// </summary>
        /// <param name="entityAssetPath"></param>
        /// <param name="groupName"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static UniTask<GameEntityLogic> ShowEntityAsync(string entityAssetPath,
                                                               string groupName,
                                                               Action onComplete = null)
        {
            var utc = new UniTaskCompletionSource<GameEntityLogic>();
            ShowEntity(entityAssetPath,
                groupName,
                entity =>
                {
                    utc.TrySetResult(entity);
                    onComplete?.Invoke();
                });
            return utc.Task;
        }

        public static async UniTask<T> ShowEntityAsync<T>(string entityAssetPath,
                                                          string groupName,
                                                          Action onComplete = null) where T : GameEntityLogic
        {
            return await ShowEntityAsync(entityAssetPath, groupName, onComplete) as T;
        }

        public static void HideEntity(int entityId)
        {
            var entitySystem = EntitySystem.Get();
            entitySystem?.HideEntity(entityId);
        }

        /// <summary>
        /// 隐藏entity
        /// </summary>
        /// <param name="entity"></param>
        public static void HideEntity(GameEntityLogic entity)
        {
            var entitySystem = EntitySystem.Get();
            entitySystem?.HideEntity(entity);
        }

        /// <summary>
        /// 如果entity正在载入,则停止
        /// </summary>
        /// <param name="id"></param>
        public static void TryStopEntityLoading(int id)
        {
            var entitySystem = EntitySystem.Get();
            if (entitySystem != null)
            {
                if (entitySystem.IsEntityLoading(id))
                {
                    entitySystem.HideEntity(id);
                }
            }
        }
    }
}
using System;
using UnityEngine;

namespace Game.Scene
{
    public class SceneElement : MonoBehaviour
    {
        /// <summary>
        /// scene element 生命周期必然是跟随monobehaviour一致的。
        /// </summary>
        internal bool isDestroyed;
        public bool IsDestroyed => isDestroyed;

        protected virtual void Awake()
        {
            isDestroyed = false;

            var sceneElementSystem = SceneElementSystem.Get();
            if (sceneElementSystem == null)
            {
                throw new Exception($"{nameof(SceneElement)} {gameObject.name} 需要系统 {nameof(SceneElementSystem)}");
            }

            sceneElementSystem.SubscribeElement(this);
        }

        protected virtual void OnDestroy()
        {
            if (Framework.IsApplicationQuitting)
            {
                return;
            }

            isDestroyed = true;
            var sceneElementSystem = SceneElementSystem.Get();
            if (sceneElementSystem == null)
            {
                throw new Exception($"{nameof(SceneElement)} {gameObject.name} 需要系统 {nameof(SceneElementSystem)}");
            }

            sceneElementSystem.UnSubscribeElement(this);
        }
    }
}
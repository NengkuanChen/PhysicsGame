using System;

namespace Game.Scene
{
    /// <summary>
    /// 继承自该类的SceneElement，在一个场景中只允许存在一个
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UniqueSceneElement<T> : SceneElement where T : SceneElement
    {
        private static T current;
        public static T Current => current;

        protected override void Awake()
        {
            base.Awake();
            if (current != null)
            {
                throw new Exception($"{nameof(SceneElement)} 是UniqueSceneElement，一个场景中只能存在一个");
            }

            current = this as T;
            if (current == null)
            {
                var thisTypeName = this.GetType().FullName;
                var genericTypeName = typeof(T).FullName;
                throw new Exception($"{thisTypeName}设置的泛型不匹配。 泛型类型： {genericTypeName}");
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            current = null;
        }
    }
}
using GameFramework.Entity;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game.Entity
{
    public class CustomEntityHelper : DefaultEntityHelper
    {
        public override IEntity CreateEntity(object entityInstance, IEntityGroup entityGroup, object userData)
        {
            var gameObject = entityInstance as GameObject;
            if (gameObject == null)
            {
                Log.Error("Entity instance is invalid.");
                return null;
            }

            var transform = gameObject.transform;
            transform.SetParent(((MonoBehaviour)entityGroup.Helper).transform);

            return gameObject.GetOrAddComponent<GameEntity>();
        }
    }
}
using System;
using Game.Entity;
using Game.Utility;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Game.GameSystem
{
    public class EntitySystem : SystemBase
    {
        public static EntitySystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as EntitySystem;
        }
        private static int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        public class ShowEntityCallbacks : IReference
        {
            public int entityId = -1;
            public Action<GameEntityLogic> onShowComplete;
            public Action onShowFailure;

            public void Clear()
            {
                onShowComplete = null;
                onShowFailure = null;
                entityId = -1;
            }
        }
        
        private int entityId;

        public EntitySystem()
        {
            var eventComponent = Framework.EventComponent;

            eventComponent.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            eventComponent.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }

        internal override void OnDisable()
        {
            base.OnDisable();

            var eventComponent = Framework.EventComponent;

            eventComponent.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            eventComponent.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }

        private void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            if (!(e is ShowEntitySuccessEventArgs args))
            {
                return;
            }

            if (!(args.UserData is ShowEntityCallbacks callbacks))
            {
                return;
            }

            callbacks.onShowComplete?.Invoke(args.Entity.Logic as GameEntityLogic);
            ReferencePool.Release(callbacks);
        }

        private void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            if (!(e is ShowEntitySuccessEventArgs args))
            {
                return;
            }

            if (!(args.UserData is ShowEntityCallbacks callbacks))
            {
                return;
            }

            callbacks.onShowFailure?.Invoke();
            ReferencePool.Release(callbacks);
        }

        public int ShowEntity(string entityAssetPath, string groupName, Action<GameEntityLogic> onShowComplete)
        {
            var callbacks = ReferencePool.Acquire<ShowEntityCallbacks>();
            callbacks.onShowComplete = onShowComplete;
            entityId++;
            var newEntityId = entityId;
            Framework.EntityComponent.ShowEntity(newEntityId, null,
                AssetPathUtility.GetEntityAssetPath(entityAssetPath), groupName, 1, callbacks);
            callbacks.entityId = newEntityId;
            return newEntityId;
        }

        public bool IsEntityLoading(int id)
        {
            return Framework.EntityComponent.IsLoadingEntity(id);
        }
        
        public void HideEntity(GameEntityLogic entityLogic)
        {
            Framework.EntityComponent.HideEntity(entityLogic.Entity);
        }

        public void HideEntity(int id)
        {
            if (id <= 0)
            {
                return;
            }

            Framework.EntityComponent.HideEntity(id);
        }
    }
}
#if UNITY_EDITOR
using UnityEditor;
using UnityGameFramework.Editor;
#endif
using System;
using UnityGameFramework.Runtime;

namespace Game.Entity
{
    public class CustomEntityComponent : EntityComponent
    {
        public override void ShowEntity(int entityId,
                                        Type entityLogicType,
                                        string entityAssetName,
                                        string entityGroupName,
                                        int priority,
                                        object userData)
        {
            m_EntityManager.ShowEntity(entityId,
                entityAssetName,
                entityGroupName,
                priority,
                ShowEntityInfo.Create(entityLogicType, userData));
        }

        protected override void OnShowEntitySuccess(object sender, GameFramework.Entity.ShowEntitySuccessEventArgs e)
        {
            m_EventComponent.FireNow(this, ShowEntitySuccessEventArgs.Create(e));
        }

        protected override void OnShowEntityFailure(object sender, GameFramework.Entity.ShowEntityFailureEventArgs e)
        {
            Log.Error($"Show entity failure, entity id '{e.EntityId.ToString()}', asset name '{e.EntityAssetName}', " +
                      $"entity group name '{e.EntityGroupName}', error message '{e.ErrorMessage}'");
            m_EventComponent.Fire(this, ShowEntityFailureEventArgs.Create(e));
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(CustomEntityComponent))]
    public class CustomEntityComponentInspector : EntityComponentInspector
    {
    }

#endif
}
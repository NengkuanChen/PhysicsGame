using System;
using GameFramework.Entity;
using UnityGameFramework.Runtime;

namespace Game.Entity
{
    public class GameEntity : UnityGameFramework.Runtime.Entity
    {
        public override void OnInit(int entityId,
                                    string entityAssetName,
                                    IEntityGroup entityGroup,
                                    bool isNewInstance,
                                    object userData)
        {
            m_Id = entityId;
            m_EntityAssetName = entityAssetName;
            if (isNewInstance)
            {
                m_EntityGroup = entityGroup;
            }
            else if (m_EntityGroup != entityGroup)
            {
                Log.Error("Entity group is inconsistent for non-new-instance entity.");
                return;
            }

            var showEntityInfo = (ShowEntityInfo) userData;

            try
            {
                if (isNewInstance)
                {
                    m_EntityLogic = gameObject.GetComponent<GameEntityLogic>();
                    if (m_EntityLogic == null)
                    {
                        Log.Error($"Entity '{entityAssetName}' can not add entity logic.");
                        return;
                    }
                    m_EntityLogic.OnInit(showEntityInfo.UserData);
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Entity '[{m_Id.ToString()}]{m_EntityAssetName}' OnInit with exception '{exception}'.");
            }
        }

        public override void OnRecycle()
        {
            try
            {
                m_EntityLogic.OnRecycle();
            }
            catch (Exception exception)
            {
                Log.Error($"Entity '[{m_Id.ToString()}]{m_EntityAssetName}' OnRecycle with exception '{exception}'.");
            }

            m_Id = 0;
        }
    }
}
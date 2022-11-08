using System;
using System.Collections.Generic;
using Game.Entity;
using Game.UI;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public class Framework : MonoBehaviour
    {
        private static Framework instance;

        public static BaseComponent BaseComponent { get; private set; }
        public static SceneComponent SceneComponent { get; private set; }
        public static ResourceComponent ResourceComponent { get; private set; }
        public static EventComponent EventComponent { get; private set; }
        public static SoundComponent SoundComponent { get; private set; }
        public static EntityComponent EntityComponent { get; private set; }
        public static UIComponent UiComponent { get; private set; }
        public static ProcedureComponent ProcedureComponent { get; private set; }

        private static bool isApplicationQuitting;
        public static bool IsApplicationQuitting => isApplicationQuitting;
        public static bool FrameworkActiviting => instance != null;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            BaseComponent = GameEntry.GetComponent<BaseComponent>();
            SceneComponent = GameEntry.GetComponent<SceneComponent>();
            ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            EventComponent = GameEntry.GetComponent<EventComponent>();
            SoundComponent = GameEntry.GetComponent<SoundComponent>();
            EntityComponent = GameEntry.GetComponent<CustomEntityComponent>();
            UiComponent = GameEntry.GetComponent<CustomUIComponent>();
            ProcedureComponent = GameEntry.GetComponent<ProcedureComponent>();
        }

        private HashSet<GameEntityLogic> lateUpdateEntities = new HashSet<GameEntityLogic>();
        private HashSet<GameEntityLogic> fixedUpdateEntities = new HashSet<GameEntityLogic>();

        public static void AddLateUpdateEntity(GameEntityLogic entityLogic)
        {
            if (entityLogic == null)
            {
                return;
            }

            if (instance.lateUpdateEntities.Contains(entityLogic))
            {
                throw new Exception($"重复添加entity{entityLogic.Name} 到late update列表");
            }

            instance.lateUpdateEntities.Add(entityLogic);
        }

        public static void RemoveLateUpdateEntity(GameEntityLogic entityLogic)
        {
            if (entityLogic == null)
            {
                return;
            }

            if (!instance.lateUpdateEntities.Contains(entityLogic))
            {
                throw new Exception($"移除不在late update列表中的entity{entityLogic.Name}");
            }

            instance.lateUpdateEntities.Remove(entityLogic);
        }

        public static void AddFixedUpdateEntity(GameEntityLogic entityLogic)
        {
            if (entityLogic == null)
            {
                return;
            }

            if (instance.fixedUpdateEntities.Contains(entityLogic))
            {
                throw new Exception($"重复添加entity{entityLogic.Name} 到fixed update列表");
            }

            instance.fixedUpdateEntities.Add(entityLogic);
        }

        public static void RemoveFixedUpdateEntity(GameEntityLogic entityLogic)
        {
            if (entityLogic == null)
            {
                return;
            }

            if (!instance.fixedUpdateEntities.Contains(entityLogic))
            {
                throw new Exception($"移除不在fixed update列表中的entity{entityLogic.Name}");
            }

            instance.fixedUpdateEntities.Remove(entityLogic);
        }

        private void LateUpdate()
        {
            if (lateUpdateEntities.Count > 0)
            {
                var deltaTime = Time.deltaTime;
                var unscaledDeltaTime = Time.unscaledDeltaTime;
                foreach (var entityLogic in lateUpdateEntities)
                {
                    entityLogic.OnLateUpdate(deltaTime, unscaledDeltaTime);
                }
            }
        }

        private void FixedUpdate()
        {
            if (fixedUpdateEntities.Count > 0)
            {
                var fixedDeltaTime = Time.fixedDeltaTime;
                var fixedUnscaledDeltaTime = Time.fixedUnscaledDeltaTime;
                foreach (var entity in fixedUpdateEntities)
                {
                    entity.OnFixedUpdate(fixedDeltaTime, fixedUnscaledDeltaTime);
                }
            }
        }

        private void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }
    }
}
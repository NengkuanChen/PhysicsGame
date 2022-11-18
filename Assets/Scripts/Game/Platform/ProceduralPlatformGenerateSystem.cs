using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Entity;
using Game.GameEvent;
using Game.GameSystem;
using Game.Scene;
using Game.Utility;
using GameFramework.Event;
using Table;
using UnityEngine;

namespace Game.PlatForm
{
    public class ProceduralPlatformGenerateSystem: SystemBase
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        public static ProceduralPlatformGenerateSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as ProceduralPlatformGenerateSystem;
        }

        private int currentTableId = 1;
        
        private List<PlatformGroupEntity> platformGroupEntities = new List<PlatformGroupEntity>();
        public List<PlatformGroupEntity> PlatformGroupEntities => platformGroupEntities;
        
        private List<PlatformGroupEntity> playerPassedEntities = new List<PlatformGroupEntity>();
        public List<PlatformGroupEntity> PlayerPassedEntities => playerPassedEntities;

        private PlatformSetting setting;



        internal override void OnEnable()
        {
            base.OnEnable();
            Framework.EventComponent.Subscribe(OnPlayerEnterPlatformGroupEventArgs.UniqueId, OnPlayerEnterPlatformGroup);
            setting = SettingUtility.PlatformSetting;
            for (int i = 0; i < setting.PlatformFront; i++)
            {
                RandomGeneratePlatformGroup().Forget();
            }
        }

        private void OnPlayerEnterPlatformGroup(object sender, GameEventArgs e)
        {
            var arg = e as OnPlayerEnterPlatformGroupEventArgs;
            if (!playerPassedEntities.Contains(arg.PlatformGroup))
            {
                playerPassedEntities.Add(arg.PlatformGroup);
            }

            var platFront = platformGroupEntities.Count - platformGroupEntities.IndexOf(arg.PlatformGroup);
            if (platFront < setting.PlatformFront)
            {
                RandomGeneratePlatformGroup().Forget();
            }


            if (playerPassedEntities.Count > setting.PlatformBehind)
            {
                var entity = playerPassedEntities[0];
                playerPassedEntities.RemoveAt(0);
                platformGroupEntities.Remove(entity);
                entity.Hide();
            }

            
        }

        public async UniTask<PlatformGroupEntity> GeneratePlatformGroup(int platformID)
        {
            var newPlatform =
                await EntityUtility.ShowEntityAsync<PlatformGroupEntity>($"PlatformGroup/{platformID}", EntityGroupName.Platform);
            newPlatform.transform.parent = ScrollRoot.Current.transform;
            
            if (platformGroupEntities.Count > 0)
            {
                newPlatform.transform.position += (platformGroupEntities[^1].ExitPoistion.position -
                                                   newPlatform.EnterPoistion.position);
            }
            else
            {
                newPlatform.transform.position += (PlatformInitialPosition.Current.transform.position -
                                                   newPlatform.EnterPoistion.position);
            }
            platformGroupEntities.Add(newPlatform);
            return newPlatform;
        }

        public async UniTask<PlatformGroupEntity> RandomGeneratePlatformGroup()
        {
            int[] currentPlatformIds = PlatformGenerationTable.GetValueOrThrErr(currentTableId).PlatformIdArray;
            int randomIndex = Random.Range(0, currentPlatformIds.Length);
            int platformId = currentPlatformIds[randomIndex];
            return await GeneratePlatformGroup(platformId);
        }

        internal override void OnDisable()
        {
            base.OnDisable();
            Framework.EventComponent.Unsubscribe(OnPlayerEnterPlatformGroupEventArgs.UniqueId, OnPlayerEnterPlatformGroup);
        }
    }
}
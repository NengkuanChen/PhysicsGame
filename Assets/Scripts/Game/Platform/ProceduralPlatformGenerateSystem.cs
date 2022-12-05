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

        private int currentTableId = 0;
        
        private List<PlatformGroupEntity> platformGroupEntities = new List<PlatformGroupEntity>();
        public List<PlatformGroupEntity> PlatformGroupEntities => platformGroupEntities;
        
        private List<PlatformGroupEntity> playerPassedEntities = new List<PlatformGroupEntity>();
        public List<PlatformGroupEntity> PlayerPassedEntities => playerPassedEntities;

        private PlatformSetting setting;

        private PlatformDebugMode debugMode;



        internal override void OnEnable()
        {
            base.OnEnable();
            Framework.EventComponent.Subscribe(OnPlayerEnterPlatformGroupEventArgs.UniqueId, OnPlayerEnterPlatformGroup);
            setting = SettingUtility.PlatformSetting;
            if (!GameDataSystem.Get().HasFinishedTutorial)
            {
                GenerateTutorialPlatformGroup().Forget();
            }
            else
            {
                RandomGeneratePlatformGroup().Forget();
            }
            for (int i = 0; i < setting.PlatformFront - 1; i++)
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
            if (debugMode != PlatformDebugMode.Disabled)
            {
                return await GenerateDebugPlatformGroup();
            }
            return await GeneratePlatformGroup(platformId);
        }

        private async UniTask<PlatformGroupEntity> GenerateDebugPlatformGroup()
        {
            PlatformGroupEntity newPlatform = null;
            switch (debugMode)
            {
                case PlatformDebugMode.Magnetic:
                    newPlatform =
                        await EntityUtility.ShowEntityAsync<PlatformGroupEntity>($"PlatformGroup/Magnetic",
                            EntityGroupName.Platform);
                    break;
                case PlatformDebugMode.FlameBoard:
                    newPlatform =
                        await EntityUtility.ShowEntityAsync<PlatformGroupEntity>("PlatformGroup/Flame",
                            EntityGroupName.Platform);
                    break;
                case PlatformDebugMode.PaperBoard:
                    newPlatform =
                        await EntityUtility.ShowEntityAsync<PlatformGroupEntity>("PlatformGroup/Paper",
                            EntityGroupName.Platform);
                    break;
                case PlatformDebugMode.WaterPool:
                    newPlatform =
                        await EntityUtility.ShowEntityAsync<PlatformGroupEntity>("PlatformGroup/WaterPool",
                            EntityGroupName.Platform);
                    break;
                case PlatformDebugMode.Wind:
                    newPlatform = await EntityUtility.ShowEntityAsync<PlatformGroupEntity>("PlatformGroup/Wind",
                        EntityGroupName.Platform);
                    break;
            }

            if (newPlatform == null)
            {
                return null;
            }
            
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



        private async UniTask<PlatformGroupEntity> GenerateTutorialPlatformGroup()
        {
            var tutorialPlatforms =
                await EntityUtility.ShowEntityAsync<PlatformGroupEntity>($"PlatformGroup/Tutorial", EntityGroupName.Platform);
            tutorialPlatforms.transform.parent = ScrollRoot.Current.transform;
            tutorialPlatforms.transform.position += (PlatformInitialPosition.Current.transform.position -
                                                     tutorialPlatforms.EnterPoistion.position);
            platformGroupEntities.Add(tutorialPlatforms);
            return tutorialPlatforms;
        }

        public async UniTask ResetScene()
        {
            foreach (var entity in platformGroupEntities)
            {
                entity.Hide();
            }
            platformGroupEntities.Clear();
            playerPassedEntities.Clear();
            if (!GameDataSystem.Get().HasFinishedTutorial)
            {
                GenerateTutorialPlatformGroup().Forget();
            }
            else
            {
                RandomGeneratePlatformGroup().Forget();
            }
            for (int i = 0; i < setting.PlatformFront - 1; i++)
            {
                RandomGeneratePlatformGroup().Forget();
            }
        }

        public void EnablePlatformDebugMode(PlatformDebugMode debugMode)
        {
            this.debugMode = debugMode;
        }
        
        internal override void OnDisable()
        {
            base.OnDisable();
            Framework.EventComponent.Unsubscribe(OnPlayerEnterPlatformGroupEventArgs.UniqueId, OnPlayerEnterPlatformGroup);
        }
    }
}
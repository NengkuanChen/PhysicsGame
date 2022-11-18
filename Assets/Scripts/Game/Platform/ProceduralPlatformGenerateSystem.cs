using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Entity;
using Game.GameSystem;
using Game.Utility;

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

        internal override void OnEnable()
        {
            base.OnEnable();
        }

        public async UniTask<PlatformGroupEntity> GeneratePlatformGroup(int platformID)
        {
            var newPlatform =
                await EntityUtility.ShowEntityAsync<PlatformGroupEntity>($"PlatformGroup/{platformID}", EntityGroupName.Platform);
            platformGroupEntities.Add(newPlatform);
            return newPlatform;
        }
    }
}
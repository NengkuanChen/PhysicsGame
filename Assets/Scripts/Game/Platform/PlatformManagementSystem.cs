using System.Collections.Generic;
using Game.GameSystem;
using Game.PlatForm;

namespace Game.PlatForm
{
    public class PlatformManagementSystem: SystemBase
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();

        internal override int ID => UniqueId;

        public static PlatformManagementSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as PlatformManagementSystem;
        }

        private List<PlatformEntity> platformQueue;
        public List<PlatformEntity> PlatformQueue => platformQueue;
        
        
    }
}
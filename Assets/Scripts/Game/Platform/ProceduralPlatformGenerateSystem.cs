using Game.GameSystem;

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
        
        
    }
}
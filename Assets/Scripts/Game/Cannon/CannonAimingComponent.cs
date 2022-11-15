using Game.Entity;

namespace Game.Cannon
{
    public class CannonAimingComponent: SpecificEntityComponentBase<CannonEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;
        
        
    }
}
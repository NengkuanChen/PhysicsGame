using Game.Entity;

namespace Game.Ball
{
    public class BallSwitchComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
        }
    }
}
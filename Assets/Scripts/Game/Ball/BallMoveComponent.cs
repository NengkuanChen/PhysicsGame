using Game.Entity;

namespace Game.Ball
{
    public class BallMoveComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();

        public override int ID => UniqueId;

        private BallSetting setting;

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            setting = OwnerEntityType.Setting;
        }
    }
}
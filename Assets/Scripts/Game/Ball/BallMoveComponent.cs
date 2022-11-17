using Game.Entity;

namespace Game.Ball
{
    public class BallMoveComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();

        public override int ID => UniqueId;

        public override bool NeedFixedUpdate => true;

        private BallSetting setting;

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            setting = OwnerEntityType.Setting;
        }

        public override void OnEntityFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityFixedUpdate(elapseSeconds, realElapseSeconds);
            OwnerEntityType.BallRigidBody.AddForce();
        }
    }
}
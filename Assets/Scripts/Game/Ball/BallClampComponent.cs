using Game.Entity;
using UnityEngine;

namespace Game.Ball
{
    public class BallClampComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;

        private BallSetting setting;

        public override bool NeedFixedUpdate => true;

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            setting = OwnerEntityType.Setting;
        }

        public override void OnEntityFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityFixedUpdate(elapseSeconds, realElapseSeconds);
            var position = OwnerTransform.position;
            if (position.x < setting.BallHorizontalRange.x || position.x > setting.BallHorizontalRange.y)
            {
                var velocity = OwnerEntityType.BallRigidBody.velocity;
                velocity.x = 0;
                OwnerEntityType.BallRigidBody.velocity = velocity;
                // OwnerEntity.transform.position =
                //     new Vector3(Mathf.Clamp(position.x, setting.BallHorizontalRange.x, setting.BallHorizontalRange.y),
                //         position.y, position.z);
                // OwnerEntityType.BallRigidBody.position = new Vector3(Mathf.Clamp(position.x, setting.BallHorizontalRange.x, setting.BallHorizontalRange.y),
                //     position.y, position.z);
            }
        }
    }
}
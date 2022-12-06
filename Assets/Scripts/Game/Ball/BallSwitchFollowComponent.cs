using Game.Entity;
using UnityEngine;

namespace Game.Ball
{
    public class BallSwitchFollowComponent: SpecificEntityComponentBase<BallSwitchEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;

        public override bool NeedLateUpdate => true;

        private Transform targetBall;

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            targetBall = BallSystem.Get()?.PlayerCurrentBall.transform;
        }

        public override void OnEntityLateUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityLateUpdate(elapseSeconds, realElapseSeconds);
            if (targetBall == null)
            {
                return;
            }

            OwnerTransform.position = targetBall.position + Vector3.back * .5f;
            OwnerEntityType.WoodBallTransform.rotation = targetBall.rotation;
            OwnerEntityType.IronBallTransform.rotation = targetBall.rotation;
        }

        public override void OnComponentDetach()
        {
            base.OnComponentDetach();
            OwnerTransform.position = new Vector3(80f, -50f, 20);
        }
    }
}
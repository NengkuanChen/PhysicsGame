using Game.Entity;
using UnityEngine;

namespace Game.Ball
{
    public class BallFixComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;

        public override bool NeedFixedUpdate => true;

        private Vector3 initialPosition;
        private Quaternion initialRotation;

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            initialPosition = OwnerTransform.position;
            initialRotation = OwnerTransform.rotation;
        }


        public override void OnEntityFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityFixedUpdate(elapseSeconds, realElapseSeconds);
            OwnerEntityType.BallRigidBody.position = initialPosition;
            OwnerEntityType.BallRigidBody.velocity = Vector3.zero;
            OwnerTransform.rotation = initialRotation;
        }
    }
}
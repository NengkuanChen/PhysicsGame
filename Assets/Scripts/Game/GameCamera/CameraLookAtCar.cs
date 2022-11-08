using Game.Entity;
using UnityEngine;

namespace Game.GameCamera
{
    public class CameraLookAtCar : SpecificEntityComponentBase<CameraEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;

        private Transform target;

        public CameraLookAtCar(Transform target)
        {
            this.target = target;
        }

        public override void OnEntityUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityUpdate(elapseSeconds, realElapseSeconds);

            if (target == null)
            {
                return;
            }

            var cameraTransform = OwnerEntityType.Camera.transform;
            cameraTransform.LookAt(target);
        }
    }
}
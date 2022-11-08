using Game.Entity;
using UnityEngine;

namespace Game.Car
{
    public class CarRotate : SpecificEntityComponentBase<CarEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;

        public override void OnEntityUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityUpdate(elapseSeconds, realElapseSeconds);
            OwnerTransform.Rotate(Vector3.up, 90 * elapseSeconds);
        }
    }
}
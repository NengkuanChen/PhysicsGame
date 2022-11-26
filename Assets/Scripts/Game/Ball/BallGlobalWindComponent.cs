using Game.Entity;
using Game.PlatForm;
using UnityEngine;

namespace Game.Ball
{
    public class BallGlobalWindComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;

        private GlobalWindZoneSetting setting;
        private float cumulateTime = 0;
        private Vector3 maxForce;

        public override bool NeedFixedUpdate => true;

        public BallGlobalWindComponent(Vector3 maxForce, GlobalWindZoneSetting setting, float cumulateTime = 0)
        {
            this.maxForce = maxForce;
            this.setting = setting;
            this.cumulateTime = cumulateTime;
        }

        public override void OnEntityFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityFixedUpdate(elapseSeconds, realElapseSeconds);
            cumulateTime += elapseSeconds;

            var curForce =
                setting.TimeWindForceCurve.Evaluate(Mathf.Clamp(cumulateTime / setting.TimeToMaxWind, 0, 1)) *
                maxForce;
            OwnerEntityType.BallRigidBody.AddForce(curForce, ForceMode.VelocityChange);
        }
    }
}
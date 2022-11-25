using Game.Entity;
using Game.PlatForm;
using UnityEngine;

namespace Game.Ball
{
    public class IronBallMagneticComponent: SpecificEntityComponentBase<IronBall>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        
        public override int ID => UniqueId;

        public override bool NeedFixedUpdate => true;

        private MagneticPlatform magneticPlatform;

        private MagneticPlatformSetting setting;

        public IronBallMagneticComponent(MagneticPlatform magneticPlatform)
        {
            this.magneticPlatform = magneticPlatform;
            setting = magneticPlatform.Setting;
        }

        public override void OnEntityFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityFixedUpdate(elapseSeconds, realElapseSeconds);
            var distance = magneticPlatform.transform.position - OwnerTransform.position;
            var force = distance.normalized *
                        (setting.ForceCurve.Evaluate(distance.magnitude / magneticPlatform.MagneticTrigger.radius) *
                         setting.MaxForce);
            OwnerEntityType.BallRigidBody.AddForce(force, ForceMode.VelocityChange);

        }
    }
}
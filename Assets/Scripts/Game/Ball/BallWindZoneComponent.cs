using Game.Entity;
using Game.GameSystem;
using Game.Utility;
using UnityEngine;
using WindZone = Game.PlatForm.WindZone;

namespace Game.Ball
{
    public class BallWindZoneComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        
        public override int ID => UniqueId;

        public override bool NeedFixedUpdate => true;
        
        private Vector3 windForce;
        
        public BallWindZoneComponent(Vector3 windForce)
        {
            this.windForce = windForce;
        }
        

        public override void OnEntityFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityFixedUpdate(elapseSeconds, realElapseSeconds);
            OwnerEntityType.BallRigidBody.AddForce(windForce, ForceMode.VelocityChange);
        }
    }
}
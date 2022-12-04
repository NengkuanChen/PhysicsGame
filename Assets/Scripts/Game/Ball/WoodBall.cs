using UnityEngine;

namespace Game.Ball
{
    public class WoodBall: BallEntity
    {
        public override void ActiveBall(Vector3 rigidBodyVelocity, Vector3 position)
        {
            base.ActiveBall(rigidBodyVelocity, position);
            AddComponent(new BallFlameDetectionComponent());
        }
    }
}
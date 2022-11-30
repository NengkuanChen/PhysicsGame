using Game.GameEvent;
using GameFramework.Event;
using UnityEngine;

namespace Game.Ball
{
    public class PlasticBall: BallEntity
    {
        public override void ActiveBall(Vector3 rigidBodyVelocity, Vector3 position)
        {
            base.ActiveBall(rigidBodyVelocity, position);
            AddComponent(new BallFlameDetectionComponent());
        }

        

        public override void DeactiveBall()
        {
            base.DeactiveBall();
        }
    }
}
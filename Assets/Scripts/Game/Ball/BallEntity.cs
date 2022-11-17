using Game.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Ball
{
    public abstract class BallEntity: GameEntityLogic
    {
        [SerializeField, Required] 
        private Rigidbody ballRigidBody;

        public Rigidbody BallRigidBody => ballRigidBody;

        [SerializeField, LabelText("Ball Setting"), Required]
        private BallSetting setting;

        public BallSetting Setting => setting;

        [SerializeField, LabelText("Ball Type")]
        private BallType ballType;
        public BallType BallType => ballType;
        
        private bool isBallActive;
        public bool IsBallActive => isBallActive;

        public virtual void ActiveBall(Vector3 rigidBodyVelocity, Vector3 position)
        {
            RemoveAllComponents();
            AddComponent(new BallMoveComponent());
            AddComponent(new BallSwitchComponent());
            ballRigidBody.velocity = rigidBodyVelocity;
            transform.position = position;
        }
        
        public virtual void DeActiveBall()
        {
            RemoveAllComponents();
            AddComponent(new BallFixComponent());
            transform.position = new Vector3(0, 0, 20);
            transform.rotation = Quaternion.identity;
        }
    }

    public enum BallType
    {
        IronBall,
        PlasticBall,
        WoodBall,
    }
}
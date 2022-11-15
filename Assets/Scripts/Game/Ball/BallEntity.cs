using Game.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Ball
{
    public class BallEntity: GameEntityLogic
    {
        [SerializeField, Required] 
        private Rigidbody ballRigidBody;

        public Rigidbody BallRigidBody => ballRigidBody;

        [SerializeField, LabelText("Ball Setting"), Required]
        private BallSetting setting;

        public BallSetting Setting => setting;
    }
}
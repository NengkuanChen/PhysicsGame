using Cysharp.Threading.Tasks;
using Game.Entity;
using Game.GameEvent;
using Game.GameSystem;
using Game.PlatForm;
using GameFramework.Event;
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

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            Framework.EventComponent.Subscribe(OnBallEnterWindZoneEventArgs.UniqueId, OnBallEnterWindZone);
        }

        public virtual void OnBallEnterWindZone(object sender, GameEventArgs e)
        {
            var arg = e as OnBallEnterWindZoneEventArgs;
            if (arg == null)
            {
                return;
            }

            if (arg.BallName != name)
            {
                return;
            }

            if (arg.IsEnter)
            {
                foreach (var windForce in arg.WindZone.Setting.WindForces)
                {
                    if (windForce.BallType == ballType)
                    {
                        // GlobalWindZoneSystem.Get().OpenWindForm(arg.WindZone.WindDirection.eulerAngles).Forget();
                        AddComponent(new BallWindZoneComponent(windForce.WindForce * arg.WindZone.WindDirection.up));
                        break;
                    }
                }
            }
            else
            {
                // GlobalWindZoneSystem.Get().CloseWindForm();
                RemoveComponent(BallWindZoneComponent.UniqueId);
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            Framework.EventComponent.Unsubscribe(OnBallEnterWindZoneEventArgs.UniqueId, OnBallEnterWindZone);
        }

        public virtual void ActiveBall(Vector3 rigidBodyVelocity, Vector3 position)
        {
            RemoveAllComponents();
            AddComponent(new BallMoveComponent());
            AddComponent(new BallSwitchComponent());
            AddComponent(new BallClampComponent());
            ballRigidBody.velocity = rigidBodyVelocity;
            transform.position = position;
        }
        
        public virtual void DeactiveBall()
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
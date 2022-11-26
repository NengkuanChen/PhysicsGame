using System;
using Cysharp.Threading.Tasks;
using Game.Ball;
using Game.GameEvent;
using Game.PlatForm;
using GameFramework.Event;
using UnityEngine;

namespace Game.GameSystem
{
    public class GlobalWindZoneSystem: SystemBase
    {
        public static int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;
        
        private bool isCurrentlyActive = false;
        public bool IsCurrentlyActive => isCurrentlyActive;
        
        private float cumulateTime = 0f;
        public float CumulateTime => cumulateTime;

        private GlobalWindZoneSetting currentActiveSetting;
        public GlobalWindZoneSetting CurrentActiveSetting => currentActiveSetting;

        private Vector3 currentWindDir;
        public Vector3 CurrentWindDir => currentWindDir;
        
        internal override void OnEnable()
        {
            base.OnEnable();
            Framework.EventComponent.Subscribe(OnGlobalWindZoneEventArgs.UniqueId, OnGlobalWindZoneEnable);
            // Framework.EventComponent.Subscribe(OnBallSwitchEventArgs.UniqueId, OnBallSwitch);
        }
        
        private void OnGlobalWindZoneEnable(object sender, GameEventArgs e)
        {
            if (isCurrentlyActive)
            {
                return;
            }
            var arg = e as OnGlobalWindZoneEventArgs;
            if (arg == null)
            {
                return;
            }
            isCurrentlyActive = true;
            var maxForce = Vector3.zero;
            var currentBall = BallSystem.Get().PlayerCurrentBall;
            foreach (var force in arg.WindZone.Setting.WindForces)
            {
                if (force.BallType == currentBall.BallType)
                {
                    maxForce = force.WindForce * arg.WindZone.WindDirection.up;
                    break;
                }
            }
            currentBall.AddComponent(new BallGlobalWindComponent(maxForce, arg.WindZone.Setting));
            WindZoneHandle(currentBall, arg.WindZone.Setting.TotalConstantTime).Forget();
        }

        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (isCurrentlyActive)
            {
                cumulateTime += elapseSeconds;
            }
        }

        private async UniTask WindZoneHandle(BallEntity ballEntity, float constTime)
        {
            isCurrentlyActive = false;
            cumulateTime = 0f;
            BallSystem.Get().PlayerCurrentBall.RemoveComponent(BallGlobalWindComponent.UniqueId);
        }

        public void OnBallSwitch(BallEntity before, BallEntity after)
        {
            before.RemoveComponent(BallGlobalWindComponent.UniqueId);
            var maxForce = Vector3.zero;
            foreach (var force in currentActiveSetting.WindForces)
            {
                if (force.BallType == after.BallType)
                {
                    maxForce = force.WindForce * currentWindDir;
                    break;
                }
            }
            after.AddComponent(new BallGlobalWindComponent(maxForce, currentActiveSetting, cumulateTime));
        }

        internal override void OnDisable()
        {
            base.OnDisable();
            Framework.EventComponent.Unsubscribe(OnGlobalWindZoneEventArgs.UniqueId, OnGlobalWindZoneEnable);
            // Framework.EventComponent.Unsubscribe(OnBallSwitchEventArgs.UniqueId, OnBallSwitch);
        }
    }
}
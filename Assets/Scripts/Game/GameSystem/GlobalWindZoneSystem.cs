using System;
using Cysharp.Threading.Tasks;
using Game.Ball;
using Game.GameEvent;
using Game.PlatForm;
using Game.Sound;
using Game.UI.Form;
using Game.Utility;
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

        private Vector3 indicatorEulerAngle;

        private UniTask currentTask;

        public static GlobalWindZoneSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as GlobalWindZoneSystem;
        }
        
        internal override void OnEnable()
        {
            base.OnEnable();
            Framework.EventComponent.Subscribe(OnGlobalWindZoneEventArgs.UniqueId, OnGlobalWindZoneEnable);
            // Framework.EventComponent.Subscribe(OnBallSwitchEventArgs.UniqueId, OnBallSwitch);
            Framework.EventComponent.Subscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
        }

        private void OnBallDead(object sender, GameEventArgs e)
        {
            if (isCurrentlyActive)
            {
                isCurrentlyActive = false;
                currentActiveSetting = null;
                currentWindDir = Vector3.zero;
                cumulateTime = 0f;
                if (UIUtility.GetForm(WindIndicatorForm.UniqueId) != null)
                {
                    CloseWindForm();
                }
                SoundSystem.Get().StopPlay(SettingUtility.SoundSet.GetAudio(0));
            }
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
            currentActiveSetting = arg.WindZone.Setting;
            var maxForce = Vector3.zero;
            var currentBall = BallSystem.Get().PlayerCurrentBall;
            currentWindDir = arg.WindZone.WindDirection.up;
            indicatorEulerAngle = arg.WindZone.WindDirection.eulerAngles;
            foreach (var force in arg.WindZone.Setting.WindForces)
            {
                if (force.BallType == currentBall.BallType)
                {
                    maxForce = force.WindForce * currentWindDir;
                    break;
                }
            }
            Log.Info(maxForce);
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
            cumulateTime = 0f;
            await OpenWindForm(indicatorEulerAngle);
            SoundSystem.Get().Play(SettingUtility.SoundSet.GetAudio(0));
            await UniTask.Delay(TimeSpan.FromSeconds(constTime));
            cumulateTime = 0f;
            if (UIUtility.GetForm(WindIndicatorForm.UniqueId) != null)
            {
                CloseWindForm();
            }
            BallSystem.Get().PlayerCurrentBall.RemoveComponent(BallGlobalWindComponent.UniqueId);
            SoundSystem.Get().StopPlay(SettingUtility.SoundSet.GetAudio(0));
            isCurrentlyActive = false;
        }

        public void OnBallSwitch(BallEntity before, BallEntity after)
        {
            if (!isCurrentlyActive)
            {
                return;
            }
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
            Log.Info(maxForce);
            after.AddComponent(new BallGlobalWindComponent(maxForce, currentActiveSetting, cumulateTime));
        }

        internal override void OnDisable()
        {
            base.OnDisable();
            Framework.EventComponent.Unsubscribe(OnGlobalWindZoneEventArgs.UniqueId, OnGlobalWindZoneEnable);
            Framework.EventComponent.Unsubscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
            // Framework.EventComponent.Unsubscribe(OnBallSwitchEventArgs.UniqueId, OnBallSwitch);
        }

        public async UniTask OpenWindForm(Vector3 windDir)
        {
            var windIndicatorForm = await UIUtility.OpenFormAsync<WindIndicatorForm>(WindIndicatorForm.UniqueId);
            windIndicatorForm.SetWindDirection(windDir);
        }

        public void CloseWindForm()
        {
            UIUtility.CloseForm(WindIndicatorForm.UniqueId);
        }
    }
}
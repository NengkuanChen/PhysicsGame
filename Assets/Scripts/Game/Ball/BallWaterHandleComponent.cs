using Game.Entity;
using Game.GameEvent;
using Game.PlatForm;
using GameFramework.Event;
using UnityEngine;

namespace Game.Ball
{
    public class BallWaterHandleComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;

        public override bool NeedFixedUpdate => true;

        private Transform waterLevelTransform;
        
        private WaterPoolPlatformSetting buoyancySetting;

        public BallWaterHandleComponent(Transform waterLevel, WaterPoolPlatformSetting setting)
        {
            waterLevelTransform = waterLevel;
            buoyancySetting = setting;
        }

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            Framework.EventComponent.Subscribe(OnBallEnterWaterEventArgs.UniqueId, OnBallExit);
        }

        private void OnBallExit(object sender, GameEventArgs e)
        {
            var args = e as OnBallEnterWaterEventArgs;
            if (args == null)
            {
                return;
            }

            if (!args.IsEnter)
            {
                RemoveSelf();
            }
        }

        public override void OnEntityFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityFixedUpdate(elapseSeconds, realElapseSeconds);
            var ballDiameter = OwnerEntityType.BallCollider.radius * 2;
            var ballTopY = OwnerTransform.position.y + ballDiameter / 2;
            var waterLevel = waterLevelTransform.position.y;
            var sunkPortion = 1 - Mathf.Clamp01(ballTopY - waterLevel) / ballDiameter;
            var buoyancyForce = buoyancySetting.DepthForceCurve.Evaluate(sunkPortion) * buoyancySetting.MaxBuoyancyForce;
            OwnerEntityType.BallRigidBody.AddForce(buoyancyForce * Vector3.up, ForceMode.Impulse);
        }
    }
}
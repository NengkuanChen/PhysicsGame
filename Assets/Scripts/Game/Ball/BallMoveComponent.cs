using Game.Entity;
using Game.GameSystem;
using Game.Utility;
using UnityEngine;

namespace Game.Ball
{
    public class BallMoveComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();

        public override int ID => UniqueId;

        public override bool NeedFixedUpdate => true;

        private BallSetting setting;

        private PlayerInputSystem inputSystem;

        private PlayerInputSetting inputSetting;

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            setting = OwnerEntityType.Setting;
            inputSetting = SettingUtility.PlayerInputSetting;
            inputSystem = PlayerInputSystem.Get();
        }

        public override void OnEntityFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnEntityFixedUpdate(elapseSeconds, realElapseSeconds);
            var inputValue = inputSystem.DeviceRotate;
            // var pushForce = setting.InputForceCurve.Evaluate(Mathf.Abs(inputValue / inputSetting.MaxDeviceInputAngle)) *
            //                 setting.PushForce;
            // OwnerEntityType.BallRigidBody.AddForce(Vector3.right * pushForce * ((inputValue > 0) ? 1 : -1),
            //     ForceMode.Force);
            
            
            
            var currentVelocity = OwnerEntityType.BallRigidBody.velocity;
            currentVelocity.x =
                setting.InputForceCurve.Evaluate(Mathf.Abs(inputValue / inputSetting.MaxDeviceInputAngle)) *
                setting.MaxHorizontalSpeed * ((inputValue > 0) ? 1 : -1);
            OwnerEntityType.BallRigidBody.velocity = currentVelocity;
            
            var curVelocity = OwnerEntityType.BallRigidBody.velocity;
            curVelocity.x = Mathf.Clamp(curVelocity.x, -setting.MaxHorizontalSpeed, setting.MaxHorizontalSpeed);
            curVelocity.y = Mathf.Clamp(curVelocity.y, -setting.MaxVerticalSpeed, setting.MaxVerticalSpeed);

        }
    }
}
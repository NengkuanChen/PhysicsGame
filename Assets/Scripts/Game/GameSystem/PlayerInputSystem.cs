using Game.GameEvent;
using Game.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

namespace Game.GameSystem
{
    public class PlayerInputSystem: SystemBase
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private PlayerInputAction playerInput;

        private float deviceRotate;
        public float DeviceRotate => deviceRotate;
        
        private PlayerInputSetting playerInputSetting;

        public static PlayerInputSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as PlayerInputSystem;
        }

        internal override void OnEnable()
        {
            base.OnEnable();
            playerInputSetting = SettingUtility.PlayerInputSetting;
            playerInput = new PlayerInputAction();
            
#if UNITY_EDITOR
            playerInput.Editor.Enable();
            playerInput.Editor.Fire.performed += OnPlayerTap;
            playerInput.Editor.BallMove.performed += OnPlayerMoveBallEditor;
#else
            playerInput.Player.Enable();
            InputSystem.EnableDevice(Gyroscope.current);
            InputSystem.EnableDevice(AttitudeSensor.current);
            playerInput.Player.BallMove.performed += OnPlayerMoveBall;
            playerInput.Player.Tap.performed += OnPlayerTap;
#endif
        }

        private void OnPlayerTap(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Framework.EventComponent.Fire(this, OnPlayerTapScreen.Create());
            }
        }

        private void OnPlayerMoveBall(InputAction.CallbackContext context)
        {
            var rot = context.ReadValue<Quaternion>();
            deviceRotate = rot.eulerAngles.x;
            deviceRotate = deviceRotate > 180 ? deviceRotate - 360 : deviceRotate;
            deviceRotate = Mathf.Clamp(deviceRotate, -playerInputSetting.MaxDeviceInputAngle,
                playerInputSetting.MaxDeviceInputAngle);
        }

        private void OnPlayerMoveBallEditor(InputAction.CallbackContext context)
        {
            deviceRotate = context.ReadValue<float>();
            Log.Info(deviceRotate);
        }

        private void OnDeviceRotate(InputAction.CallbackContext context)
        {
            // Log.Info(context.ReadValue<Quaternion>().eulerAngles);
        }
        
    }
}
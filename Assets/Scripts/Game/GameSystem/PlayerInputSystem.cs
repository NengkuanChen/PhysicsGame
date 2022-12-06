using Game.GameEvent;
using Game.Utility;
using GameFramework.Event;
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

        private bool isInputLogEnable = false;

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
            Framework.EventComponent.Subscribe(OnEditorPlayerMoveBallEventArgs.UniqueId, OnPlayerMoveBallEditor);
            playerInput.Editor.Enable();
            playerInput.Editor.ChangeBallEditor.performed += OnPlayerChangeBallEditor;
#else
            playerInput.Player.Enable();
            InputSystem.EnableDevice(Gyroscope.current);
            InputSystem.EnableDevice(AttitudeSensor.current);
            Log.Info(InputSystem.GetDevice<AttitudeSensor>().name);
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

        public void EnableInputLog()
        {
            isInputLogEnable = true;
        }

        private void OnPlayerMoveBall(InputAction.CallbackContext context)
        {
            var rot = context.ReadValue<Quaternion>();
            deviceRotate = rot.eulerAngles.y % 360;
            deviceRotate = deviceRotate > 180 ? deviceRotate - 360 : deviceRotate;
            deviceRotate = Mathf.Clamp(deviceRotate, -playerInputSetting.MaxDeviceInputAngle,
                playerInputSetting.MaxDeviceInputAngle);
            if (isInputLogEnable)
            {
                Log.Info($"Device Rotate: {deviceRotate}");
            }
        }

        private void OnPlayerMoveBallEditor(object sender, GameEventArgs e)
        {
            var arg = e as OnEditorPlayerMoveBallEventArgs;
            deviceRotate = arg.Axis;
        }
        
        private void OnPlayerChangeBallEditor(InputAction.CallbackContext context)
        {
            Framework.EventComponent.Fire(this, OnControlFormHitEventArgs.Create());
        }

        private void OnDeviceRotate(InputAction.CallbackContext context)
        {
            // Log.Info(context.ReadValue<Quaternion>().eulerAngles);
            
        }
        
        public void RebootGyroscope()
        {
            InputSystem.DisableDevice(Gyroscope.current);
            InputSystem.EnableDevice(Gyroscope.current);
        }
        

        internal override void OnDisable()
        {
            base.OnDisable();
#if UNITY_EDITOR
            Framework.EventComponent.Unsubscribe(OnEditorPlayerMoveBallEventArgs.UniqueId, OnPlayerMoveBallEditor);
#endif
        }
    }
}
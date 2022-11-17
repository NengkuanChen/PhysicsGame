using Game.GameEvent;
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

        public static PlayerInputSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as PlayerInputSystem;
        }

        internal override void OnEnable()
        {
            base.OnEnable();
            playerInput = new PlayerInputAction();
            
#if UNITY_EDITOR
            playerInput.Editor.Enable();
            playerInput.Editor.Fire.performed += OnPlayerFire;
            playerInput.Editor.BallMove.performed += OnPlayerMoveBall;
#else
            playerInput.Player.Enable();
            InputSystem.EnableDevice(Gyroscope.current);
            InputSystem.EnableDevice(AttitudeSensor.current);
            playerInput.Player.BallMove.performed += OnDeviceRotate;
#endif
        }

        private void OnPlayerFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Framework.EventComponent.Fire(this, OnPlayerAttemptingFireCannonEventArgs.Create());
            }
        }

        private void OnPlayerMoveBall(InputAction.CallbackContext context)
        {
            var rot = context.ReadValue<Quaternion>();
            deviceRotate = rot.eulerAngles.y;
            deviceRotate = deviceRotate > 180 ? deviceRotate - 360 : deviceRotate;
        }

        private void OnDeviceRotate(InputAction.CallbackContext context)
        {
            Log.Info(context.ReadValue<Quaternion>().eulerAngles);
        }
        
    }
}
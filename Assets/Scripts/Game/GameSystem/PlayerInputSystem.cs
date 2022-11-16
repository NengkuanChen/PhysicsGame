using Game.GameEvent;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameSystem
{
    public class PlayerInputSystem: SystemBase
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private PlayerInputAction playerInput;

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
#endif
            
            playerInput.Player.BallMove.performed += OnDeviceRotate;
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
            Log.Info($"{context.ReadValue<float>()}");
        }

        private void OnDeviceRotate(InputAction.CallbackContext context)
        {
            Log.Info(context.valueType);
        }

        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            var deviceRot = playerInput.Player.BallMove.ReadValue<Vector3>();
            Debug.Log(deviceRot);
        }
    }
}
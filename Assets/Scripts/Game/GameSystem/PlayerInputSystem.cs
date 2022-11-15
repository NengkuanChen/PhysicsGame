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
            playerInput.Enable();
            playerInput.Player.Fire.performed += OnPlayerFire;
            playerInput.Player1.BallMove.performed += OnDeviceRotate;
        }

        private void OnPlayerFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Framework.EventComponent.Fire(this, OnPlayerAttemptingFireCannonEventArgs.Create());
            }
        }

        private void OnDeviceRotate(InputAction.CallbackContext context)
        {
            Debug.Log(context.valueType);
        }
        
        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            
        }
    }
}
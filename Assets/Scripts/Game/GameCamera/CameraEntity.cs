using Game.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.GameCamera
{
    public class CameraEntity : GameEntityLogic
    {
        [SerializeField, Required]
        private Camera camera;
        public Camera Camera => camera;
    }
}
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.GameSystem
{
    [CreateAssetMenu(menuName = "PlayerInputSetting", fileName = "PlayerInputSetting", order = 0)]
    public class PlayerInputSetting : ScriptableObject
    {
        [SerializeField, LabelText("Maximum Device Input Angle")]
        private float maxDeviceInputAngle = 45f;
        public float MaxDeviceInputAngle => maxDeviceInputAngle;
    }
}
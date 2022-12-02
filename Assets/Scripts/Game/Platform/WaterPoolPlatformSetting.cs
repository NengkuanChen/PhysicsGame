using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    [CreateAssetMenu(fileName = "WaterPoolPlatformSetting", menuName = "WaterPoolPlatformSetting", order = 0)]
    public class WaterPoolPlatformSetting : ScriptableObject
    {
        [SerializeField, LabelText("Max Buoyancy Force")]
        private float maxBuoyancyForce = 20f;
        public float MaxBuoyancyForce => maxBuoyancyForce;
        
        [SerializeField, LabelText("Depth - Force Curve")]
        private AnimationCurve depthForceCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve DepthForceCurve => depthForceCurve;
    }
}
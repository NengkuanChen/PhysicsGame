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
        
        [SerializeField, LabelText("Water Drag Force Max")]
        private float waterDragForceMax = 30f;
        
        public float WaterDragForceMax => waterDragForceMax;
        
        [SerializeField, LabelText("Max Drag At What Speed")]
        private float maxDragAtSpeed = 10f;

        public float MaxDragAtSpeed => maxDragAtSpeed;
        
        [SerializeField, LabelText("Speed Drag Force Curve")]
        private AnimationCurve speedDragForceCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve SpeedDragForceCurve => speedDragForceCurve;


    }
}
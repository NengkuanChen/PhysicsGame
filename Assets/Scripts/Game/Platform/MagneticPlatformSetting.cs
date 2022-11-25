using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    [CreateAssetMenu(fileName = "MagneticPlatformSetting", menuName = "MagneticPlatformSetting", order = 0)]
    public class MagneticPlatformSetting : ScriptableObject
    {
        [SerializeField, LabelText("Max Drag Force")]
        private float maxForce;

        public float MaxForce => maxForce;
        
        [SerializeField, LabelText("Distance - Force Curve")]
        private AnimationCurve forceCurve = AnimationCurve.Linear(0, 1, 1, 0);

        public AnimationCurve ForceCurve => forceCurve;
        
    }
}
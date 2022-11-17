using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Ball
{
    [CreateAssetMenu(fileName = "BallSetting", menuName = "BallSetting", order = 0)]
    public class BallSetting : ScriptableObject
    {
        [SerializeField, LabelText("Max Push Force")] 
        private float pushForce;

        public float PushForce => pushForce;
        
        [SerializeField, LabelText("Max Horizontal Speed")]
        private float maxHorizontalSpeed;
        public float MaxHorizontalSpeed => maxHorizontalSpeed;
        
        [SerializeField, LabelText("Max Vertical Speed")]
        private float maxVerticalSpeed;
        public float MaxVerticalSpeed => maxVerticalSpeed;
        
        [SerializeField, LabelText("Input - Force Curve")]
        private AnimationCurve inputForceCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve InputForceCurve => inputForceCurve;
        
        [SerializeField, LabelText("Next Ball")]
        private BallType nextBall;
        public BallType NextBall => nextBall;
    }
}
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    [CreateAssetMenu(fileName = "GlobalWindZoneSetting", menuName = "GlobalWindZoneSetting", order = 0)]
    public class GlobalWindZoneSetting : WindZoneSetting
    {
        [SerializeField, LabelText("Time to Max Wind")]
        private float timeToMaxWind = 3f;
        public float TimeToMaxWind => timeToMaxWind;
        
        [SerializeField, LabelText("Total Constant Time")]
        private float totalConstantTime = 5f;
        public float TotalConstantTime => totalConstantTime;
        
        [SerializeField, LabelText("Time-WindForce Curve")]
        private AnimationCurve timeWindForceCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve TimeWindForceCurve => timeWindForceCurve;
        
        
    }
}
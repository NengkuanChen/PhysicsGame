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
        
        
    }
}
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Sound
{
    [CreateAssetMenu(fileName = "SoundSetting", menuName = "Settings/SoundSetting", order = 0)]
    public class SoundSetting : ScriptableObject
    {
        [SerializeField, Min(0f), LabelText("落地音效最少落地速度")]
        private float groundedAudioRequiresMinGroundedSpeed = 5f;
        public float GroundedAudioRequiresMinGroundedSpeed => groundedAudioRequiresMinGroundedSpeed;

    }
}
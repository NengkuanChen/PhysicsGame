using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Setting
{
    [CreateAssetMenu(fileName = "GameSetting", menuName = "Settings/GameSetting", order = 0)]
    public class GameSetting : ScriptableObject
    {
        [SerializeField]
        private AnimationCurve newScoreBounceCurve;
        public AnimationCurve NewScoreBounceCurve => newScoreBounceCurve;

        [SerializeField] 
        private AnimationCurve scoreShowCurve;
        public AnimationCurve ScoreShowCurve => scoreShowCurve;
    }
}
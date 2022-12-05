using System;
using System.Collections.Generic;
using Game.GameSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    [CreateAssetMenu(fileName = "PlatformSetting", menuName = "PlatformSetting", order = 0)]
    public class PlatformSetting : ScriptableObject
    {
        [SerializeField, LabelText("Platform Scroll Max Speed"), Required, Min(0.01f)]
        private float scrollSpeed;

        public float ScrollSpeed => scrollSpeed;

        [SerializeField, LabelText("Platform Scroll Acceleration"), Required, Min(0.01f)]
        private float scrollAcceleration;

        public float ScrollAcceleration => scrollAcceleration;

        [SerializeField, LabelText("At least how many platform groups in front of player"), Required, Min(1)]
        private int platformFront;
        public int PlatformFront => platformFront;
        
        [SerializeField, LabelText("At most how many platform groups behind of player"), Required, Min(1)]
        private int platformBehind;
        public int PlatformBehind => platformBehind;
        
        [SerializeField, LabelText("Scroll Battle Acceleration")]
        private float scrollBattleAcceleration;
        public float ScrollBattleAcceleration => scrollBattleAcceleration;
    }
}
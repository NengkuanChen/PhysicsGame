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
    }
}
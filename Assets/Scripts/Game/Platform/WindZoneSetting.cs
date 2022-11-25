using System;
using System.Collections.Generic;
using Game.Ball;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    [CreateAssetMenu(fileName = "WindZoneSetting", menuName = "WindZoneSetting", order = 0)]
    public class WindZoneSetting : ScriptableObject
    {
        [SerializeField, LabelText("Wind Forces")]
        private List<BallWindForce> windForces;
        public List<BallWindForce> WindForces => windForces;

        [Serializable]
        public class BallWindForce
        {
            [SerializeField, Required]
            private BallType ballType;
            public BallType BallType => ballType;
            
            [SerializeField, LabelText("Wind Force")]
            private float windForce = 1f;
            public float WindForce => windForce;
        }
    }
}
using System;
using Game.Ball;
using Game.GameEvent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    public class MagneticPlatform: PlatformEntity
    {
        [SerializeField, Required, LabelText("Magnetic Range")]
        private SphereCollider magneticTrigger;
        
        public SphereCollider MagneticTrigger => magneticTrigger;

        [SerializeField, Required, LabelText("Magnetic Platform Setting")]
        private MagneticPlatformSetting setting;

        public MagneticPlatformSetting Setting => setting;


        [Button(ButtonSizes.Large, Name = "Quick Set Up")]
        public void QuickSetup()
        {
            if (magneticTrigger == null)
            {
                magneticTrigger = gameObject.AddComponent<SphereCollider>();
                magneticTrigger.radius = 3;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (BallSystem.Get().playerCurrentBall is IronBall)
                {
                    Framework.EventComponent.Fire(this, OnBallEnterMagneticFieldEventArgs.Create(this, true));
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (BallSystem.Get().playerCurrentBall is IronBall)
                {
                    Framework.EventComponent.Fire(this, OnBallEnterMagneticFieldEventArgs.Create(this, false));
                }
            }
        }
    }
}
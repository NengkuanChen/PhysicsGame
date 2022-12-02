using System;
using Game.GameEvent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    public class WaterPoolPlatform: PlatformEntity
    {
        [SerializeField, LabelText("Water Pool Platform Setting")]
        private WaterPoolPlatformSetting setting;
        public WaterPoolPlatformSetting Setting => setting;


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Framework.EventComponent.Fire(this, OnBallEnterWaterEventArgs.Create(true, other.name));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Framework.EventComponent.Fire(this, OnBallEnterWaterEventArgs.Create(false, other.name));
            }
        }
    }
}
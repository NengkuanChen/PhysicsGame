using System;
using Game.GameEvent;
using Game.Sound;
using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    public class BreakablePlatform: PlatformEntity
    {
        [SerializeField, LabelText("What hit speed the platform breaks")]
        private float speedBreak = 5f;
        public float SpeedBreak => speedBreak;

        public override void OnPlatformShow()
        {
            base.OnPlatformShow();
            gameObject.SetActive(true);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.other.CompareTag("Player"))
            {
                Log.Info($"HitSpeed: {collision.relativeVelocity}, {Mathf.Abs(collision.relativeVelocity.y) > speedBreak}");
                Framework.EventComponent.Fire(this, OnBallHitBreakablePlatformEventArgs.Create(Mathf.Abs(collision.relativeVelocity.y) > speedBreak));
            }
        }
        
        public void PlatformBreak()
        {
            SoundSystem.Get()?.Play(SettingUtility.SoundSet.GetAudio(5));
            gameObject.SetActive(false);
        }
    }
}
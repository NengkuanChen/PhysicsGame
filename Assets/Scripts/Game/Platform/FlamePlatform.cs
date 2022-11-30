using System;
using Game.GameEvent;
using UnityEngine;

namespace Game.PlatForm
{
    public class FlamePlatform: PlatformEntity
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Framework.EventComponent.Fire(this, OnBallHitFlamePlatformEventArgs.Create());
            }
        }
    }
}
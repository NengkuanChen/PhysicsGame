using System;
using Game.GameEvent;
using UnityEngine;

namespace Game.Scene
{
    public class BallBoundTrigger: SceneElement
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Framework.EventComponent.Fire(this, OnBallDeadEventArgs.Create());
            }
        }
    }
}
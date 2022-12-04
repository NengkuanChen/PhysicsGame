using System;
using Game.GameSystem;
using UnityEngine;

namespace Game.PlatForm
{
    public class TutorialFinishTrigger: PlatformEntity
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameDataSystem.Get().HasFinishedTutorial = true;
            }
        }
    }
}
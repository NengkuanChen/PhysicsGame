using System;
using UnityEngine;

namespace Game.PlatForm
{
    public class PlatformEnterTrigger: MonoBehaviour
    {
        public PlatformGroupEntity OwnerPlatformGroup;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OwnerPlatformGroup.OnPlayerEnter();
            }
        }
    }
}
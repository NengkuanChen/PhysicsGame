using System;
using Game.Entity;
using UnityEngine;

namespace Game.PlatForm
{
    public abstract class PlatformEntity: MonoBehaviour
    {

        [SerializeField] 
        private Rigidbody platformRigidbody;

        public Rigidbody PlatformRigidbody => platformRigidbody;
        
        public virtual void OnPlatformShow()
        {
            
        }
        
        public virtual void OnPlatformHide()
        {
            
        }

        public virtual void OnPlayerHitPlatform()
        {
            
        }
        
        public virtual void OnPlayerExitPlatform()
        {
            
        }
        
        
        
    }
}
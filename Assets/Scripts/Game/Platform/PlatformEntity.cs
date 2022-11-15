using System;
using Game.Entity;
using UnityEngine;

namespace Game.PlatForm
{
    public abstract class PlatformEntity: GameEntityLogic
    {

        [SerializeField] 
        private Rigidbody platformRigidbody;

        public Rigidbody PlatformRigidbody => platformRigidbody;
        
        public virtual void OnPlatformLoad()
        {
            
        }

        public virtual void OnPlatformShoot()
        {
            
        }

        public virtual void OnPlatformHit()
        {
            
        }

        public virtual void OnPlatformPlaced()
        {
            
        }
        
    }
}
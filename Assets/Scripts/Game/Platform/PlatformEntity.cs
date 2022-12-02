using System;
using Game.Entity;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.PlatForm
{
    public abstract class PlatformEntity: MonoBehaviour
    {

        [SerializeField, LabelText("Random Position On X Axis")]
        private bool isRandomX = false;
        public bool IsRandomX => isRandomX;
        
        [SerializeField, LabelText("Random X Range")]
        private Vector2 randomXRange = new Vector2(-4, 4);
        public Vector2 RandomXRange => randomXRange;
        
        public virtual void OnPlatformShow()
        {
            if (IsRandomX)
            {
                var randomX = Random.Range(randomXRange.x, randomXRange.y);
                transform.position = new Vector3(randomX, transform.position.y, transform.position.z);
            }
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
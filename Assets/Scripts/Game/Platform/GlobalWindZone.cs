using System;
using Game.GameEvent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    public class GlobalWindZone: PlatformEntity
    {
        [SerializeField, LabelText("Entry Trigger"), Required]
        private BoxCollider entryTrigger;
        
        [SerializeField, LabelText("Global Wind Setting"), Required]
        private GlobalWindZoneSetting setting;
        
        public GlobalWindZoneSetting Setting => setting;
        
        [SerializeField, LabelText("Wind Direction"), Required]
        private Transform windDirection;
        public Transform WindDirection => windDirection;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Framework.EventComponent.Fire(this, OnGlobalWindZoneEventArgs.Create(this));
            }
        }
    }
}
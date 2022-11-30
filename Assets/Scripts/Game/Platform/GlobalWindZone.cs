using System;
using Game.GameEvent;
using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

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
        
        [SerializeField, LabelText("Is Random Direction"), Required]
        private bool isRandomDirection;
        
        [SerializeField, LabelText("Is Random 2-way"), Required]
        private bool isRandom2way;


        public override void OnPlatformShow()
        {
            base.OnPlatformShow();
            if (isRandom2way)
            {
                windDirection.eulerAngles = new Vector3(0, 0, 90 + Random.Range(0, 2) * 180);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Framework.EventComponent.Fire(this, OnGlobalWindZoneEventArgs.Create(this));
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (windDirection == null || entryTrigger == null)
            {
                return;
            }
            
            DebugExtension.DebugArrow(windDirection.position, windDirection.up * 3, Color.red);
            DebugExtension.DrawBounds(entryTrigger.bounds, Color.magenta);
        }

        [Button(ButtonSizes.Large, Name = "Quick Setup")]
        public void QuickSetup()
        {
            if (windDirection == null)
            {
                windDirection = new GameObject("WindDir").transform;
                windDirection.parent = transform;
                windDirection.localPosition = Vector3.zero;
            }

            if (entryTrigger == null)
            {
                entryTrigger = gameObject.AddComponent<BoxCollider>();
                entryTrigger.isTrigger = true;
                entryTrigger.size = new Vector3(20, 1, 20);
            }
        }
#endif
        
    }
}
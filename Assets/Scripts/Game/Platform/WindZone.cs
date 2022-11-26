using System;
using Game.GameEvent;
using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.PlatForm
{
    public class WindZone: PlatformEntity
    {
        [SerializeField, LabelText("Wind Direction"), Required]
        private Transform windDirection;
        
        public Transform WindDirection => windDirection;
        
        [SerializeField, LabelText("Wind Zone Trigger"), Required]
        private BoxCollider windZoneTrigger;
        
        [SerializeField, LabelText("Wind Zone Setting"), Required]
        private WindZoneSetting setting;
        public WindZoneSetting Setting => setting;
        
        [SerializeField, LabelText("Is Random Direction"), Required]
        private bool isRandomDirection;
        
        [SerializeField, LabelText("Is Random 4-way"), Required]
        private bool isRandom4Way;

        public override void OnPlatformShow()
        {
            base.OnPlatformShow();
            if (isRandomDirection)
            {
                windDirection.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            }

            if (isRandom4Way)
            {
                windDirection.rotation = Quaternion.Euler(0, 0, Random.Range(0, 4) * 90);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Framework.EventComponent.Fire(this, OnBallEnterWindZoneEventArgs.Create(this, true, other.name));
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Framework.EventComponent.Fire(this, OnBallEnterWindZoneEventArgs.Create(this, false, other.name));
            }
        }


#if UNITY_EDITOR
        [Button(ButtonSizes.Large, Name = "Quick Setup")]
        public void QuickSetup()
        {
            if (windDirection == null)
            {
                windDirection = new GameObject("WindDir").transform;
                windDirection.parent = transform;
                windDirection.localPosition = Vector3.zero;
            }

            if (windZoneTrigger == null)
            {
                windZoneTrigger = gameObject.AddComponent<BoxCollider>();
                windZoneTrigger.isTrigger = true;
            }
        }

        private void OnDrawGizmos()
        {
            if (windDirection == null || windZoneTrigger == null)
            {
                return;
            }
            
            DebugExtension.DebugArrow(windDirection.position, windDirection.up * 3, Color.red);
            DebugExtension.DrawBounds(windZoneTrigger.bounds, Color.magenta);
        }
#endif
        
        
    }
}
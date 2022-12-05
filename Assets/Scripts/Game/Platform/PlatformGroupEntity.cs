using System.Collections.Generic;
using Game.Entity;
using Game.GameEvent;
using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    public class PlatformGroupEntity: GameEntityLogic
    {
        [SerializeField, LabelText("All Platforms")] 
        private List<PlatformEntity> allPlatformEntities;

        [SerializeField, LabelText("All Platforms Entities")]
        private List<PlatformEntity> AllPlatformEntities => allPlatformEntities;

        [SerializeField, LabelText("Enter Line")]
        private Transform enterPoistion;
        public Transform EnterPoistion => enterPoistion;
        
        [SerializeField, LabelText("Exit Line")]
        private Transform exitPoistion;
        public Transform ExitPoistion => exitPoistion;

        [SerializeField, LabelText("Enter Detector Trigger")]
        private PlatformEnterTrigger enterTrigger;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            foreach (var platform in allPlatformEntities)
            {
                platform.OnPlatformShow();
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            foreach (var platform in allPlatformEntities)
            {
                platform.OnPlatformHide();
            }
        }

        public void OnPlayerEnter()
        {
            Framework.EventComponent.Fire(this, OnPlayerEnterPlatformGroupEventArgs.Create(this));
        }
        
#if UNITY_EDITOR
        [Button(ButtonSizes.Large, Name = "Quick Setup")]
        private void QuickSetup()
        {
            allPlatformEntities.Clear();
            allPlatformEntities.AddRange(GetComponentsInChildren<PlatformEntity>());
            if (enterPoistion == null)
            {
                enterPoistion = new GameObject("EnterPosition").transform;
                enterPoistion.SetParent(transform);
                enterPoistion.localPosition = Vector3.zero;
            }
            if (exitPoistion == null)
            {
                exitPoistion = new GameObject("ExitPosition").transform;
                exitPoistion.SetParent(transform);
                exitPoistion.localPosition = Vector3.zero;
            }

            if (enterTrigger == null)
            {
                var enterTriggerCollider = new GameObject("EnterTrigger").AddComponent<BoxCollider>();
                enterTriggerCollider.isTrigger = true;
                enterTriggerCollider.transform.SetParent(enterPoistion);
                enterTriggerCollider.transform.localPosition = Vector3.zero;
                enterTriggerCollider.size = new Vector3(20, 1, 5);
                enterTrigger = enterTriggerCollider.gameObject.AddComponent<PlatformEnterTrigger>();
                enterTrigger.OwnerPlatformGroup = this;
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (enterPoistion != null)
            {
                DebugExtension.DrawLine(enterPoistion.position - Vector3.right * 5, enterPoistion.position + Vector3.right * 5, Color.green);
            }
            if (exitPoistion != null)
            {
                DebugExtension.DrawLine(exitPoistion.position - Vector3.right * 5, exitPoistion.position + Vector3.right * 5, Color.red);
            }
        }
#endif
        
    }
}
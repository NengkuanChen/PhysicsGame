using System.Collections.Generic;
using Game.Entity;
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
        private List<PlatformEntity> AllPlatformEntities;

        [SerializeField, LabelText("Enter Line")]
        private Transform enterPoistion;
        public Transform EnterPoistion => enterPoistion;
        
        [SerializeField, LabelText("Exit Line")]
        private Transform exitPoistion;
        public Transform ExitPoistion => exitPoistion;

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
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (enterPoistion != null)
            {
                DebugExtension.DrawLine(enterPoistion.position - Vector3.right * 10, enterPoistion.position + Vector3.right * 10, Color.green);
            }
            if (exitPoistion != null)
            {
                DebugExtension.DrawLine(exitPoistion.position - Vector3.right * 10, exitPoistion.position + Vector3.right * 10, Color.red);
            }
        }
#endif
        
    }
}
using System;
using Game.GameEvent;
using Game.PlatForm;
using Game.Utility;
using GameFramework.Event;
using UnityEngine;

namespace Game.Scene
{
    public class ScrollRoot: UniqueSceneElement<ScrollRoot>, ISceneElementUpdate, ISceneElementOnSceneReset
    {
        private bool hasStart = false;
        private float currentSpeed = 0f;
        private Vector3 initialPos;

        private PlatformSetting setting;
        
        protected override void Awake()
        {
            base.Awake();
            Framework.EventComponent.Subscribe(OnGameStartEventArgs.UniqueId, OnGameStart);
            setting = SettingUtility.PlatformSetting;
            initialPos = transform.position;
        }

        private void OnGameStart(object o, GameEventArgs e)
        {
            hasStart = true;
        }

        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (hasStart)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, setting.ScrollSpeed,
                    setting.ScrollAcceleration * elapseSeconds);
                transform.position += Vector3.up * currentSpeed * elapseSeconds;
            }
        }
        
        public void StopScroll()
        {
            hasStart = false;
        }

        

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Framework.EventComponent.Unsubscribe(OnGameStartEventArgs.UniqueId, OnGameStart);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DebugExtension.DrawWireSphere(transform.position, Color.magenta);
        }
#endif
        public void OnSceneReset()
        {
            hasStart = false;
            currentSpeed = 0f;
            transform.position = initialPos;
        }
    }
}
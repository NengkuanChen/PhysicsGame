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
        private float scrollSpeed = 0f;
        private bool isStop = false;

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
            scrollSpeed = setting.ScrollSpeed;
        }

        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (hasStart && !isStop)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, scrollSpeed,
                    setting.ScrollAcceleration * elapseSeconds);
                scrollSpeed += setting.ScrollBattleAcceleration * elapseSeconds;
                transform.position += Vector3.up * currentSpeed * elapseSeconds;
            }
        }
        
        public void StopScroll()
        {
            hasStart = false;
        }

        public void PauseScroll()
        {
            isStop = true;
        }
        
        public void ContinueScroll()
        {
            isStop = false;
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
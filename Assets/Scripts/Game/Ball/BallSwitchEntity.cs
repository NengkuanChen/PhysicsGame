using System;
using DG.Tweening;
using Game.Entity;
using Game.GameEvent;
using GameFramework.Event;
using UnityEngine;

namespace Game.Ball
{
    public class BallSwitchEntity: GameEntityLogic
    {
        [SerializeField] 
        private Transform ironBallTransform;
        public Transform IronBallTransform => ironBallTransform;
        
        [SerializeField]
        private Transform woodBallTransform;
        public Transform WoodBallTransform => woodBallTransform;
        
        [SerializeField]
        private float switchTime = .5f;
        public float SwitchTime => switchTime;

        [SerializeField] 
        private Transform switchPlane;
        public Transform SwitchPlane => switchPlane;

        [SerializeField] 
        private float ironBallX = -.8f;
        public float IronBallX => ironBallX;
        
        [SerializeField]
        private float woodBallX = .5f;
        public float WoodBallX => woodBallX;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            Framework.EventComponent.Subscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
        }

        private void OnBallDead(object sender, GameEventArgs e)
        {
            switchPlane.DOKill();
            RemoveComponent(BallSwitchFollowComponent.UniqueId);
        }

        public void OnBallSwitch(BallType ballType)
        {
            RemoveComponent(BallSwitchEffectComponent.UniqueId);
            RemoveComponent(BallSwitchFollowComponent.UniqueId);
            AddComponent(new BallSwitchEffectComponent(ballType));
            AddComponent(new BallSwitchFollowComponent());
        }

        public void Reset()
        {
            transform.position = new Vector3(80f, -50f, 20);
            var planeLocalPos = switchPlane.localPosition;
            planeLocalPos.x = ironBallX;
            switchPlane.localPosition = planeLocalPos;
        }


        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            Framework.EventComponent.Unsubscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
        }
    }
}
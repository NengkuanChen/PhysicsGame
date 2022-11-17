using System;
using Cysharp.Threading.Tasks;
using Game.Entity;
using Game.GameEvent;
using Game.GameSystem;
using Game.Scene;
using Game.Utility;
using GameFramework.Event;
using UnityEngine;

namespace Game.Ball
{
    public class BallSystem: SystemBase
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private IronBall currentIronBall;
        public IronBall CurrentIronBall => currentIronBall;
        
        private WoodBall currentWoodBall;
        public WoodBall CurrentWoodBall => currentWoodBall;
        
        private PlasticBall currentPlasticBall;
        public PlasticBall CurrentPlasticBall => currentPlasticBall;

        public BallEntity playerCurrentBall;
        public BallEntity PlayerCurrentBall => playerCurrentBall;

        public static BallSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as BallSystem;
        }

        internal override void OnEnable()
        {
            base.OnEnable();
            Framework.EventComponent.Subscribe(OnGameStartEventArgs.UniqueId, OnGameStart);
        }

        public async UniTask<BallEntity> LoadBallEntityAsync()
        {
            if (currentIronBall != null)
            {
                currentIronBall.Hide();
            }

            if (currentWoodBall)
            {
                currentWoodBall.Hide();
            }
            
            if (currentPlasticBall)
            {
                currentPlasticBall.Hide();
            }

            currentIronBall = await EntityUtility.ShowEntityAsync<IronBall>("Ball/IronBall", EntityGroupName.Ball);
            // currentWoodBall = await EntityUtility.ShowEntityAsync<WoodBall>("Ball/WoodBall", EntityGroupName.Ball);
            // currentPlasticBall = await EntityUtility.ShowEntityAsync<PlasticBall>("Ball/PlasticBall", EntityGroupName.Ball);
            playerCurrentBall = currentIronBall;
            return playerCurrentBall;
        }

        public void OnWaitingGameStart()
        {
            if (ScrollRoot.Current == null)
            {
                throw new Exception("Scroll Root doesn't exist!!!");
            }
            currentIronBall.transform.parent = ScrollRoot.Current.transform;
            currentIronBall.transform.localPosition = Vector3.zero;
            currentIronBall.transform.rotation = Quaternion.identity;
            currentIronBall.AddComponent(new BallFixComponent());
        }

        public void OnGameStart(object o, GameEventArgs args)
        {
            playerCurrentBall.RemoveComponent(BallFixComponent.UniqueId);
            playerCurrentBall.AddComponent(new BallMoveComponent());
        }
        
        internal override void OnDisable()
        {
            base.OnDisable();
            Framework.EventComponent.Unsubscribe(OnGameStartEventArgs.UniqueId, OnGameStart);
        }
    }
}
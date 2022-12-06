using System;
using System.Collections.Generic;
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

        private BallEntity playerCurrentBall;
        public BallEntity PlayerCurrentBall => playerCurrentBall;
        
        private List<BallEntity> allBalls = new List<BallEntity>();
        public List<BallEntity> AllBalls => allBalls;

        private BallSwitchEntity ballSwitchEntity;

        public static BallSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as BallSystem;
        }

        internal override void OnEnable()
        {
            base.OnEnable();
            Framework.EventComponent.Subscribe(OnGameStartEventArgs.UniqueId, OnGameStart);
            Framework.EventComponent.Subscribe(OnBallSwitchEventArgs.UniqueId, OnBallSwitch);
            Framework.EventComponent.Subscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
        }

        private void OnBallDead(object sender, GameEventArgs e)
        {
            var ballDeadEventArgs = e as OnBallDeadEventArgs;
            if (ballDeadEventArgs == null)
            {
                return;
            }

            if (ballDeadEventArgs.IsBurned)
            {
                PlayBallBurnParticle().Forget();

            }
            else
            {
                playerCurrentBall.DeactiveBall();
            }
        }

        private async UniTask PlayBallBurnParticle()
        {
            var particle =
                await EntityUtility.ShowEntityAsync<BallFireDestroyParticleEntity>("Ball/Fireball",
                    EntityGroupName.Ball);
            particle.transform.position = playerCurrentBall.transform.position;
            playerCurrentBall.DeactiveBall();
            particle.FireParticle.Play();
            await UniTask.WaitUntil(() => !particle.FireParticle.isPlaying);
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            particle.Hide();
        }

        private void OnBallSwitch(object sender, GameEventArgs e)
        {
            Log.Info("BallSwitch");
            var arg = e as OnBallSwitchEventArgs;
            if (arg.BallType == playerCurrentBall.BallType)
            {
                return;
            }
            foreach (var ball in allBalls)
            {
                if (ball.BallType == arg.BallType)
                {
                    // ball.transform.position
                    ball.ActiveBall(playerCurrentBall.BallRigidBody.velocity, playerCurrentBall.transform.position);
                    playerCurrentBall.DeactiveBall();
                    GlobalWindZoneSystem.Get().OnBallSwitch(playerCurrentBall, ball);
                    playerCurrentBall = ball;
                    ballSwitchEntity.OnBallSwitch(arg.BallType);
                    break;
                }
            }
        }

        public async UniTask<BallEntity> LoadBallEntityAsync()
        {
            foreach (var ball in allBalls)
            {
                ball.Hide();
            }

            var loadTasks = new List<UniTask<BallEntity>>
            {
                EntityUtility.ShowEntityAsync<BallEntity>("Ball/IronBall", EntityGroupName.Ball),
                EntityUtility.ShowEntityAsync<BallEntity>("Ball/WoodBall", EntityGroupName.Ball),
                EntityUtility.ShowEntityAsync<BallEntity>("Ball/PlasticBall", EntityGroupName.Ball),
            };
            var results = await UniTask.WhenAll(loadTasks);
            foreach (var ball in results)
            {
                ball.DeactiveBall();
                allBalls.Add(ball);
            }
            currentIronBall = allBalls[0] as IronBall;
            playerCurrentBall = currentIronBall;
            ballSwitchEntity = await EntityUtility.ShowEntityAsync<BallSwitchEntity>("Ball/BallSwitcher", EntityGroupName.Ball);
            
            return playerCurrentBall;
        }
        
        public void Reset()
        {
            foreach (var ball in allBalls)
            {
                ball.DeactiveBall();
            }
            ballSwitchEntity.Reset();
            playerCurrentBall = currentIronBall;
            // playerCurrentBall.ActiveBall(Vector3.zero, ScrollRoot.Current.transform.position);
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
            if (!currentIronBall.ContainComponent(BallFixComponent.UniqueId))
            {
                currentIronBall.AddComponent(new BallFixComponent());
            }
        }

        public void OnGameStart(object o, GameEventArgs args)
        {
            playerCurrentBall.ActiveBall(Vector3.zero, playerCurrentBall.transform.position);
        }

        
        
        internal override void OnDisable()
        {
            base.OnDisable();
            Framework.EventComponent.Unsubscribe(OnGameStartEventArgs.UniqueId, OnGameStart);
            Framework.EventComponent.Unsubscribe(OnBallSwitchEventArgs.UniqueId, OnBallSwitch);
            Framework.EventComponent.Unsubscribe(OnBallDeadEventArgs.UniqueId, OnBallDead);
        }
    }
}
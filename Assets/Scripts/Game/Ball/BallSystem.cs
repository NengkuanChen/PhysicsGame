using System;
using Cysharp.Threading.Tasks;
using Game.Entity;
using Game.GameSystem;
using Game.Scene;
using Game.Utility;
using UnityEngine;

namespace Game.Ball
{
    public class BallSystem: SystemBase
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private BallEntity currentBall;
        public BallEntity CurrentBall => currentBall;

        public static BallSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as BallSystem;
        }


        public async UniTask<BallEntity> LoadBallEntityAsync()
        {
            if (currentBall != null)
            {
                currentBall.Hide();
            }

            currentBall = await EntityUtility.ShowEntityAsync<BallEntity>("Ball/BallEntity", EntityGroupName.Ball);
            return currentBall;
        }

        public void OnWaitingGameStart()
        {
            if (ScrollRoot.Current == null)
            {
                throw new Exception("Scroll Root doesn't exist!!!");
            }
            currentBall.transform.parent = ScrollRoot.Current.transform;
            currentBall.transform.localPosition = Vector3.zero;
            currentBall.transform.rotation = Quaternion.identity;
            currentBall.AddComponent(new BallFixComponent());
        }
    }
}
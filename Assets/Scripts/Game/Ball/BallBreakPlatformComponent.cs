using Game.Entity;
using Game.GameEvent;
using Game.PlatForm;
using GameFramework.Event;
using UnityEngine;

namespace Game.Ball
{
    public class BallBreakPlatformComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();

        public override int ID => UniqueId;

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            Framework.EventComponent.Subscribe(OnBallHitBreakablePlatformEventArgs.UniqueId, OnBallHitBreakablePlatform);
        }

        private void OnBallHitBreakablePlatform(object sender, GameEventArgs e)
        {
            var breakablePlatform = sender as BreakablePlatform;
            if (OwnerEntityType.BallType == BallType.WoodBall)
            {
                if ((e as OnBallHitBreakablePlatformEventArgs).IsExceedSpeed)
                {
                    breakablePlatform.PlatformBreak();
                }
            }
            else if (OwnerEntityType.BallType == BallType.IronBall)
            {
                breakablePlatform.PlatformBreak();
            }
        }

        public override void OnComponentDetach()
        {
            base.OnComponentDetach();
            Framework.EventComponent.Unsubscribe(OnBallHitBreakablePlatformEventArgs.UniqueId, OnBallHitBreakablePlatform);
        }
    }
}
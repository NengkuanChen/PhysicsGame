using Game.Entity;
using Game.GameEvent;
using Game.Sound;
using Game.Utility;
using GameFramework.Event;

namespace Game.Ball
{
    public class BallFlameDetectionComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            Framework.EventComponent.Subscribe(OnBallHitFlamePlatformEventArgs.UniqueId, OnBallHitFlamePlatform);
        }
        
        private void OnBallHitFlamePlatform(object sender, GameEventArgs e)
        {
            SoundSystem.Get().Play(SettingUtility.SoundSet.GetAudio(6));
            Framework.EventComponent.Fire(this, OnBallDeadEventArgs.Create(true));
        }


        public override void OnComponentDetach()
        {
            base.OnComponentDetach();
            Framework.EventComponent.Unsubscribe(OnBallHitFlamePlatformEventArgs.UniqueId, OnBallHitFlamePlatform);
        }
    }
}
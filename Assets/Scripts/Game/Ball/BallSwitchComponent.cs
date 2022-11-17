using Game.Entity;
using Game.GameEvent;
using GameFramework.Event;

namespace Game.Ball
{
    public class BallSwitchComponent: SpecificEntityComponentBase<BallEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;
        
        

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            Framework.EventComponent.Subscribe(OnControlFormHitEventArgs.UniqueId, OnControlFormHit);
        }

        private void OnControlFormHit(object sender, GameEventArgs e)
        {
            Framework.EventComponent.Fire(this, OnBallSwitchEventArgs.Create());
        }


        public override void OnComponentDetach()
        {
            base.OnComponentDetach();
            Framework.EventComponent.Unsubscribe(OnControlFormHitEventArgs.UniqueId, OnControlFormHit);
        }
    }
}
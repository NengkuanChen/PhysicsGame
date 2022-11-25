using Game.GameEvent;
using GameFramework.Event;

namespace Game.Ball
{
    public class IronBall: BallEntity
    {
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            Framework.EventComponent.Subscribe(OnBallEnterMagneticFieldEventArgs.UniqueId, OnBallEnterMagneticField);
        }
        
        private void OnBallEnterMagneticField(object sender, GameEventArgs e)
        {
            var args = e as OnBallEnterMagneticFieldEventArgs;
            if (args == null)
            {
                return;
            }

            if (args.IsEnter)
            {
                AddComponent(new IronBallMagneticComponent(args.MagneticPlatform));
            }
            else
            {
                RemoveComponent(IronBallMagneticComponent.UniqueId);
            }
        }


        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            Framework.EventComponent.Unsubscribe(OnBallEnterMagneticFieldEventArgs.UniqueId, OnBallEnterMagneticField);
        }
    }
}
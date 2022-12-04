using Game.Ball;
using Game.GameEvent;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class MaterialShowImage: StaticUIElement
    {
        [SerializeField] 
        private BallType ballType;

        [SerializeField] 
        private Image showImage;

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            var currentBallType = BallSystem.Get().PlayerCurrentBall.BallType;
            showImage.enabled = currentBallType == ballType;
            Framework.EventComponent.Subscribe(OnBallSwitchEventArgs.UniqueId, OnBallSwitch);
        }

        private void OnBallSwitch(object sender, GameEventArgs e)
        {
            var arg = (OnBallSwitchEventArgs) e;
            showImage.enabled = arg.BallType == ballType;
        }

        public override void OnClose()
        {
            base.OnClose();
            Framework.EventComponent.Unsubscribe(OnBallSwitchEventArgs.UniqueId, OnBallSwitch);
        }
    }
}
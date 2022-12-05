using Game.GameEvent;
using Game.GameSystem;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class MuteButton: StaticUIElement
    {
        [SerializeField] 
        private ToggleButton muteToggleButton;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            muteToggleButton.OnClick.AddListener((() =>
            {
                GameDataSystem.Get().IsMute = !GameDataSystem.Get().IsMute;
            }));
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            if (GameDataSystem.Get().IsMute)
            {
                muteToggleButton.On();
            }
            else
            {
                muteToggleButton.Off();
            }
            Framework.EventComponent.Subscribe(OnAudioStatusChangedEventArgs.UniqueId, OnAudioStatusChanged);
        }

        private void OnAudioStatusChanged(object sender, GameEventArgs e)
        {
            var ne = (OnAudioStatusChangedEventArgs) e;
            if (ne.IsEnable)
            {
                muteToggleButton.Off();
            }
            else
            {
                muteToggleButton.On();
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            Framework.EventComponent.Unsubscribe(OnAudioStatusChangedEventArgs.UniqueId, OnAudioStatusChanged);
        }
    }
}
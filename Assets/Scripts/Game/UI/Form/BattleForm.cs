using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Ball;
using Game.GameEvent;
using GameFramework.Event;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class BattleForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;

        [SerializeField, LabelText("TimeShow")]
        private TextMeshProUGUI timeShowText;
        
        [SerializeField, LabelText("PauseButton")]
        private Button pauseButton;
        
        [SerializeField, LabelText("InitialPosition")]
        private RectTransform initialPosition;
        
        [SerializeField, LabelText("EndPosition")]
        private RectTransform endPosition;
        
        [SerializeField, LabelText("RootTransform")]
        private RectTransform rootTransform;
        
        static BattleForm()
        {
            UIConfig.RegisterForm(UniqueId, nameof(BattleForm), UiDepth.Common);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            pauseButton.onClick.AddListener((() =>
            {
                Framework.EventComponent.Fire(this, OnGamePauseEventArgs.Create(true));
                pauseButton.interactable = false;
            }));
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            rootTransform.position = initialPosition.position;
            rootTransform.DOMove(endPosition.position, .5f);
            Framework.EventComponent.Subscribe(OnGamePauseEventArgs.UniqueId, OnGamePause);
        }

        private void OnGamePause(object sender, GameEventArgs e)
        {
            var args = (OnGamePauseEventArgs) e;
            if (!args.IsPause)
            {
                pauseButton.interactable = true;
            }
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UpdateSurvivedTime();
        }

        private void UpdateSurvivedTime()
        {
            if (GameEvaluationSystem.Get() == null)
            {
                return;
            }
            var survivedTime = Mathf.CeilToInt(GameEvaluationSystem.Get().CurrentSurvivalTime);
            var survivedTimeText = $"{Mathf.CeilToInt(survivedTime / 60):D2}:{survivedTime % 60:D2}";
            timeShowText.text = survivedTimeText;
        }

        public async UniTaskVoid MoveOutForm()
        {
            rootTransform.DOMove(initialPosition.position, .5f).OnComplete(() =>
            {
                CloseSelf();
            });
        }
        
        
        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            Framework.EventComponent.Unsubscribe(OnGamePauseEventArgs.UniqueId, OnGamePause);
        }
    }
}
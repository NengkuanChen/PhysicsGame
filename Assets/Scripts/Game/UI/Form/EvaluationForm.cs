using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Ball;
using Game.GameSystem;
using Game.Utility;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class EvaluationForm: GameUIFormLogic
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int FormType => UniqueId;

        [SerializeField, LabelText("Continue Button")]
        private Button continueButton;
        
        [SerializeField, LabelText("Evaluation Animator")]
        private Animator evaluationAnimator;
        
        [SerializeField, LabelText("Highest Score Text")]
        private TextMeshProUGUI highestScoreText;
        
        [SerializeField, LabelText("Current Score Text")]
        private TextMeshProUGUI currentScoreText;

        [SerializeField, LabelText("New Score Text")]
        private Image newScoreText;
        
        private bool hasFinished = false;
        public bool HasFinished => hasFinished;
        
        private int currentHighScore = 0;

        private bool hasRefreshedScore = false;

        private float currentScore = 0f;
        
        static EvaluationForm()
        {
            UIConfig.RegisterForm(UniqueId,  nameof(EvaluationForm), UiDepth.Common);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            continueButton.onClick.AddListener((() =>
            {
                MoveOut().Forget();
            }));
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            continueButton.interactable = false;
            hasFinished = false;
            evaluationAnimator.Play("GameOver");
            currentScoreText.text = "00:00";
            currentHighScore = Mathf.CeilToInt(GameDataSystem.Get().HighestScore);
            highestScoreText.text = $"{currentHighScore / 60:D2}:{currentHighScore % 60:D2}";
            currentScore = GameEvaluationSystem.Get().CurrentSurvivalTime;
            hasRefreshedScore = currentScore > currentHighScore;
            newScoreText.transform.localScale = Vector3.zero;
            StartEvaluate().Forget();
        }

        private async UniTaskVoid StartEvaluate()
        {
            var setting = SettingUtility.GameSetting;
            await UniTask.Delay(700);
            var score = 0f;
            DOVirtual.Float(score, currentScore, 2f, (value) =>
            {
                score = value;
                currentScoreText.text = $"{Mathf.CeilToInt(score) / 60:D2}:{Mathf.CeilToInt(score) % 60:D2}";
            }).SetEase(Ease.OutExpo);
            
            currentScoreText.DOScale(1.2f, 0.3f).SetEase(setting.ScoreShowCurve);
            await UniTask.Delay(2200);
            var highScoreShow = (float)currentHighScore;
            if (hasRefreshedScore)
            {
                DOVirtual.Float(highScoreShow, currentScore, 1.5f, (value) =>
                {
                    highScoreShow = value;
                    highestScoreText.text = $"{Mathf.CeilToInt(highScoreShow) / 60:D2}:{Mathf.CeilToInt(highScoreShow) % 60:D2}";
                }).SetEase(setting.ScoreShowCurve).OnComplete((() =>
                {
                    newScoreText.transform.DOScale(1.3f, 0.5f).SetEase(setting.NewScoreBounceCurve);
                }));
                await UniTask.Delay(1500);
            }
            continueButton.interactable = true;
        }
        
        public async UniTaskVoid MoveOut()
        {
            evaluationAnimator.Play("MoveOut");
            await UniTask.Delay(500);
            hasFinished = true;
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            hasFinished = true;
        }
    }
}
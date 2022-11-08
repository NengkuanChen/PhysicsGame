// using System;
// using DG.Tweening;
// using Game.GameEvent;
// using Game.GameSystem;
// using GameFramework.Event;
// using Sirenix.OdinInspector;
// using TMPro;
// using UnityEngine;
//
// namespace Game.UI.Form
// {
//     public class CoinForm : GameUIFormLogic
//     {
//         public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
//         public override int FormType => UniqueId;
//         
//         static CoinForm()
//         {
//             UIConfig.RegisterForm(UniqueId, nameof(CoinForm), UiDepth.Coin);
//         }
//
//         [SerializeField, Required]
//         private TextMeshProUGUI coinCountText;
//         [SerializeField, Required]
//         private TextMeshProUGUI diamondCountText;
//         // [SerializeField, Required]
//         // private Button settingButton;
//
//         [SerializeField, Required, Min(0f)]
//         private float scaleDownWhenCoinCountChanged = .5f;
//         [SerializeField, Required, Min(0f)]
//         private float scaleDurationWhenCoinCountChanged = .4f;
//
//         [SerializeField, Required]
//         private RectTransform burstCoinTemplate;
//         [SerializeField, Required]
//         private RectTransform burstDiamondTemplate;
//
//         [BoxGroup("获取动画"), SerializeField, Required]
//         private RectTransform burstCoinEndPoint;
//         [BoxGroup("获取动画"), SerializeField, Required]
//         private RectTransform burstDiamondEndPoint;
//         [BoxGroup("获取动画"), SerializeField, LabelText("金币数量"), Min(1)]
//         private int burstCoinCount = 10;
//         [BoxGroup("获取动画"), SerializeField, LabelText("每个金币飞行时间"), Min(.1f)]
//         private float burstCoinAnimTime = .7f;
//         [BoxGroup("获取动画"), SerializeField, LabelText("金币生成间隔时间"), Min(0f)]
//         private float burstCoinSpawnInterval = .05f;
//         [BoxGroup("获取动画"), SerializeField, LabelText("金币生成范围"), Min(0f)]
//         private float burstCoinStartSpawnRange = 1f;
//         [BoxGroup("获取动画"), SerializeField, LabelText("金币生成控制点范围"), Min(0f)]
//         private float burstCoinStartControlPointRange = 2f;
//         [BoxGroup("获取动画"), SerializeField, LabelText("金币目标范围"), Min(0f)]
//         private float burstCoinEndRange = 1f;
//         [BoxGroup("获取动画"), SerializeField, LabelText("金币目标控制点范围"), Min(0f)]
//         private float burstCoinEndControlPointRange = 2f;
//
//         private BurstItemAnimator burstCoinAnimator;
//         private BurstItemAnimator burstDiamondAnimator;
//
//         protected override void OnInit(object userData)
//         {
//             base.OnInit(userData);
//             burstCoinAnimator = new BurstItemAnimator(burstCoinTemplate);
//             burstDiamondAnimator = new BurstItemAnimator(burstDiamondTemplate);
//             // settingButton.onClick.AddListener(() => UIUtility.OpenForm(SettingForm.UniqueId));
//         }
//
//         protected override void OnOpen(object userData)
//         {
//             base.OnOpen(userData);
//
//             Framework.EventComponent.Subscribe(OnCoinCountChangedEventArgs.UniqueId, OnCoinCountChanged);
//             Framework.EventComponent.Subscribe(OnDiamondCountChangedEventArgs.UniqueId, OnDiamondCountChanged);
//
//             var gameDataSystem = GameDataSystem.Get();
//             coinCountText.text = gameDataSystem.CoinCount.ToString();
//             diamondCountText.text = gameDataSystem.DiamondCount.ToString();
//         }
//
//         protected override void OnClose(bool isShutdown, object userData)
//         {
//             base.OnClose(isShutdown, userData);
//             Framework.EventComponent.Unsubscribe(OnCoinCountChangedEventArgs.UniqueId, OnCoinCountChanged);
//             Framework.EventComponent.Unsubscribe(OnDiamondCountChangedEventArgs.UniqueId, OnDiamondCountChanged);
//         }
//
//         private void OnCoinCountChanged(object sender, GameEventArgs e)
//         {
//             if (e is OnCoinCountChangedEventArgs args)
//             {
//                 coinCountText.text = args.Count.ToString();
//                 var textTransform = coinCountText.rectTransform;
//                 DOTween.Kill(textTransform);
//                 textTransform.localScale = Vector3.one * scaleDownWhenCoinCountChanged;
//                 textTransform.DOScale(1f, scaleDurationWhenCoinCountChanged).SetEase(Ease.OutBack);
//             }
//         }
//
//         private void OnDiamondCountChanged(object sender, GameEventArgs e)
//         {
//             if (e is OnDiamondCountChangedEventArgs args)
//             {
//                 diamondCountText.text = args.Count.ToString();
//                 var textTransform = diamondCountText.rectTransform;
//                 DOTween.Kill(textTransform);
//                 textTransform.localScale = Vector3.one * scaleDownWhenCoinCountChanged;
//                 textTransform.DOScale(1f, scaleDurationWhenCoinCountChanged).SetEase(Ease.OutBack);
//             }
//         }
//
//         public void BurstCoinAndDiamondAnimation(int coinCount,
//                                                  int diamondCount,
//                                                  Vector3 burstPoint,
//                                                  Action onComplete = null)
//         {
//             if (coinCount <= 0 && diamondCount <= 0)
//             {
//                 onComplete?.Invoke();
//                 return;
//             }
//
//             var gameDataSystem = GameDataSystem.Get();
//             // SoundUtility.PlaySound(1);
//
//             var needCompleteCount = 0;
//             needCompleteCount += coinCount > 0 ? 1 : 0;
//             needCompleteCount += diamondCount > 0 ? 1 : 0;
//
//             void onOneAnimationComplete()
//             {
//                 needCompleteCount--;
//                 if (needCompleteCount == 0)
//                 {
//                     onComplete?.Invoke();
//                 }
//             }
//
//             if (coinCount > 0)
//             {
//                 burstCoinAnimator.PlayAnimation(burstCoinCount,
//                     burstPoint,
//                     burstCoinStartSpawnRange,
//                     burstCoinStartControlPointRange,
//                     burstCoinEndPoint.position,
//                     burstCoinEndRange,
//                     burstCoinEndControlPointRange,
//                     burstCoinAnimTime,
//                     burstCoinSpawnInterval,
//                     () =>
//                     {
//                         gameDataSystem.IncreaseCoin(coinCount);
//                         onOneAnimationComplete();
//                     });
//             }
//
//             if (diamondCount > 0)
//             {
//                 burstDiamondAnimator.PlayAnimation(burstCoinCount,
//                     burstPoint,
//                     burstCoinStartSpawnRange,
//                     burstCoinStartControlPointRange,
//                     burstDiamondEndPoint.position,
//                     burstCoinEndRange,
//                     burstCoinEndControlPointRange,
//                     burstCoinAnimTime,
//                     burstCoinSpawnInterval,
//                     () =>
//                     {
//                         gameDataSystem.IncreaseDiamond(diamondCount);
//                         onOneAnimationComplete();
//                     });
//             }
//         }
//     }
// }
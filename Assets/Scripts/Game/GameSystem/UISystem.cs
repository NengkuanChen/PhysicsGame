using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.GameEvent;
using Game.UI;
using Game.Utility;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Game.GameSystem
{
    public class UISystem : SystemBase
    {
        public static UISystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as UISystem;
        }

        private static int UniqueId = UniqueIdGenerator.GetUniqueId();
        internal override int ID => UniqueId;

        private HashSet<int> openingFormTypeSet = new HashSet<int>();
        private Dictionary<int, GameUIFormLogic> openedUiFormDic = new Dictionary<int, GameUIFormLogic>();

        private int fullScreenCoverFormOpenCount;
        public bool HaveFullScreenCoverFormOpened => fullScreenCoverFormOpenCount > 0;

        private class WaitingOpenFormInfo
        {
            public int formType;
            public Action<UIFormLogic> onOpenCallback;
            public object userData;
        }

        /// <summary>
        /// 等待打开的UI界面
        /// 队列中第一个元素必然是打开窗口，当该窗口关闭后，再出列
        /// </summary>
        private Queue<WaitingOpenFormInfo> waitingOpenFormInfoQueue = new Queue<WaitingOpenFormInfo>();

        public UISystem()
        {
            var eventComponent = Framework.EventComponent;

            eventComponent.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUiFormSuccess);
            eventComponent.Subscribe(OpenUIFormFailureEventArgs.EventId, OnOpenUiFormFailure);
        }

        internal override void OnDisable()
        {
            base.OnDisable();
            var eventComponent = Framework.EventComponent;

            eventComponent.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUiFormSuccess);
            eventComponent.Unsubscribe(OpenUIFormFailureEventArgs.EventId, OnOpenUiFormFailure);
        }

        private void OnOpenUiFormSuccess(object sender, GameEventArgs e)
        {
            if (!(e is OpenUIFormSuccessEventArgs args))
            {
                return;
            }

            if (!(args.UserData is OpenFormData openFormData))
            {
                return;
            }

            if (openingFormTypeSet.Contains(openFormData.formType))
            {
                openingFormTypeSet.Remove(openFormData.formType);
            }

            var gameForm = args.UIForm.Logic as GameUIFormLogic;
            if (gameForm == null)
            {
                throw new Exception($"打开面板 {args.UIForm.gameObject.name} 无法获取到{nameof(GameUIFormLogic)}类型");
            }

            openedUiFormDic.Add(openFormData.formType, gameForm);

            SortOpenedFormZPosition();

            var uiAssetInfo = UIConfig.UIAssetInfoDic[openFormData.formType];
            if (uiAssetInfo.coverFullScreen)
            {
                fullScreenCoverFormOpenCount++;
            }

            //open here
            gameForm.OnCustomOpen(openFormData.openUserData);

            openFormData.onComplete?.Invoke(gameForm);
            Framework.EventComponent.Fire(this, OnFormOpenedEventArgs.Create(gameForm, openFormData.formType));
            ReferencePool.Release(openFormData);
        }

        private void OnOpenUiFormFailure(object sender, GameEventArgs e)
        {
            if (!(e is OpenUIFormFailureEventArgs args))
            {
                return;
            }

            if (!(args.UserData is OpenFormData openFormData))
            {
                return;
            }

            if (openingFormTypeSet.Contains(openFormData.formType))
            {
                openingFormTypeSet.Remove(openFormData.formType);
            }

            openFormData.onFailure?.Invoke();
            ReferencePool.Release(openFormData);

            Log.Error($"打开UI失败 {args.ErrorMessage}");
        }

        public void OpenForm(int formType, Action<UIFormLogic> onOpenComplete, object userData)
        {
            if (IsFormOpenedOrOpening(formType))
            {
                UIConfig.GetUiAssetNameAndDepth(formType, out var formName, out _);
                throw new Exception($"重复打开UI界面： {formName}");
            }

            openingFormTypeSet.Add(formType);

            if (UIConfig.GetUiAssetNameAndDepth(formType, out var assetName, out var uiDepth))
            {
                var openUIData = ReferencePool.Acquire<OpenFormData>();
                openUIData.formType = formType;
                openUIData.openUserData = userData;
                openUIData.onComplete = onOpenComplete;

                Framework.UiComponent.OpenUIForm(AssetPathUtility.GetUIFormAssetPath(assetName),
                    uiDepth.ToString(),
                    2,
                    openUIData);
            }
            else
            {
                throw new Exception($"无法打开UI: {formType}");
            }
        }

        public void OpenFormInQueue(int formType, Action<UIFormLogic> onOpenComplete, object userData)
        {
            waitingOpenFormInfoQueue.Enqueue(new WaitingOpenFormInfo
            {
                formType = formType,
                onOpenCallback = onOpenComplete,
                userData = userData
            });
            if (waitingOpenFormInfoQueue.Count == 1)
            {
                OpenForm(formType, onOpenComplete, userData);
            }
        }

        public void CloseForm(int formType)
        {
            if (openingFormTypeSet.Contains(formType))
            {
                Log.Error($"正在打开的界面无法关闭 form type {formType}, 可以在打开界面回调中关闭界面");
                return;
            }

            if (openedUiFormDic.TryGetValue(formType, out var gameUIForm))
            {
                var uiComponent = Framework.UiComponent;
                uiComponent.CloseUIForm(gameUIForm.UIForm);
                openedUiFormDic.Remove(formType);

                SortOpenedFormZPosition();

                var uiAssetInfo = UIConfig.UIAssetInfoDic[formType];
                if (uiAssetInfo.coverFullScreen)
                {
                    fullScreenCoverFormOpenCount--;
                    if (fullScreenCoverFormOpenCount < 0)
                    {
                        fullScreenCoverFormOpenCount = 0;
                        Log.Error("关闭的全屏UI比打开的多");
                    }
                }

                var onFormClosedEventArgs = ReferencePool.Acquire<OnFormClosedEventArgs>();
                onFormClosedEventArgs.closedFormType = formType;
                Framework.EventComponent.Fire(this, onFormClosedEventArgs);
            }

            if (waitingOpenFormInfoQueue.Count > 0 && waitingOpenFormInfoQueue.Peek().formType == formType)
            {
                waitingOpenFormInfoQueue.Dequeue();
                if (waitingOpenFormInfoQueue.Count > 0)
                {
                    var formInfoToOpen = waitingOpenFormInfoQueue.Peek();
                    /*
                     * 同一个窗口,无法在关闭的当帧再次打开,需要等一帧
                     */
                    if (formInfoToOpen.formType == formType)
                    {
                        OpenSameFormAgainAsync(formInfoToOpen.formType,
                                formInfoToOpen.onOpenCallback,
                                formInfoToOpen.userData)
                            .Forget();
                    }
                    else
                    {
                        OpenForm(formInfoToOpen.formType, formInfoToOpen.onOpenCallback, formInfoToOpen.userData);
                    }
                }
            }
        }

        private async UniTaskVoid OpenSameFormAgainAsync(int formType,
                                                         Action<UIFormLogic> openedCallback,
                                                         Object userData)
        {
            while (IsFormOpenedOrOpening(formType))
            {
                await UniTask.Yield();
            }

            await UniTask.Yield();

            OpenForm(formType, openedCallback, userData);
        }

        public bool IsFormOpened(int formType)
        {
            return openedUiFormDic.ContainsKey(formType);
        }

        public bool IsFormOpenedOrOpening(int formType)
        {
            return openedUiFormDic.ContainsKey(formType) || openingFormTypeSet.Contains(formType);
        }

        public T GetForm<T>(int formType) where T : UIFormLogic
        {
            if (openedUiFormDic.TryGetValue(formType, out var uiForm))
            {
                return uiForm as T;
            }

            return null;
        }

        public UIFormLogic GetForm(int formType)
        {
            if (openedUiFormDic.TryGetValue(formType, out var uiForm))
            {
                return uiForm;
            }

            return null;
        }

        private List<GameUIFormLogic> cacheFormList = new List<GameUIFormLogic>();

        private void SortOpenedFormZPosition()
        {
            cacheFormList.Clear();

            cacheFormList.AddRange(openedUiFormDic.Values);

            cacheFormList.Sort((aForm, bForm) => aForm.SortingOrder - bForm.SortingOrder);
            //按顺序 由远到近
            var uiCameraFarClipPlane = UICamera.Current.Camera.farClipPlane - 1f;
            var eachFormDistance = uiCameraFarClipPlane / cacheFormList.Count;

            for (var i = 0; i < cacheFormList.Count; i++)
            {
                cacheFormList[i].Canvas.planeDistance = uiCameraFarClipPlane - i * eachFormDistance;
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.GameSystem;
using Game.UI;
using Game.UI.Form;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Game.Utility
{
    public static class UIUtility
    {
        public static void OpenForm(int formType, Action<UIFormLogic> onComplete = null, object userData = null)
        {
            var uiSystem = UISystem.Get();
            uiSystem?.OpenForm(formType, onComplete, userData);
        }

        public static void OpenForm(int formType, object userData)
        {
            OpenForm(formType, null, userData);
        }

        public static void OpenFormInQueue(int formType, Action<UIFormLogic> onComplete = null, object userData = null)
        {
            var uiSystem = UISystem.Get();
            uiSystem?.OpenFormInQueue(formType, onComplete, userData);
        }

        public static void OpenFormInQueue(int formType, object userData)
        {
            OpenFormInQueue(formType, null, userData);
        }

        public static async UniTask<UIFormLogic> OpenFormAsync(int formType, object userData = null)
        {
            UIFormLogic openedFormLogic = null;
            OpenForm(formType, logic => openedFormLogic = logic, userData);
            while (openedFormLogic == null)
            {
                await Task.Yield();
            }

            return openedFormLogic;
        }

        public static async UniTask<T> OpenFormAsync<T>(int formType, object userData = null) where T : UIFormLogic
        {
            var openedForm = await OpenFormAsync(formType, userData);
            return openedForm as T;
        }
        
        public static async UniTask<UIFormLogic> OpenFormOrWaitingFormOpen(int formType, object userData = null)
        {
            if (IsFormOpenedOrOpening(formType))
            {
                UIFormLogic form = null;
                while (true)
                {
                    form = GetForm(formType);
                    if (form != null)
                    {
                        break;
                    }
                    await UniTask.Yield();
                }

                return form;
            }

            return await OpenFormAsync(formType, userData);
        }
        
        public static async UniTask<T> OpenFormOrWaitingFormOpen<T>(int formType, object userData = null) where T : GameUIFormLogic
        {
            var form = await OpenFormOrWaitingFormOpen(formType,userData);
            return form as T;
        }

        public static async UniTask<T> WaitingFormOpen<T>(int formType) where T : GameUIFormLogic
        {
            UIFormLogic form = null;
            while (true)
            {
                form = GetForm(formType);
                if (form != null)
                {
                    break;
                }
                await UniTask.Yield();
            }

            return form as T;
        }
        
        public static UIFormLogic GetForm(int formType)
        {
            var uiSystem = UISystem.Get();
            return uiSystem?.GetForm(formType);
        }

        public static T GetForm<T>(int formType) where T : UIFormLogic
        {
            var uiSystem = UISystem.Get();
            return uiSystem?.GetForm(formType) as T;
        }

        public static void CloseForm(int formType)
        {
            var uiSystem = UISystem.Get();
            uiSystem?.CloseForm(formType);
        }

        public static bool IsFormOpened(int formType)
        {
            var uiSystem = UISystem.Get();
            if (uiSystem != null)
            {
                return uiSystem.IsFormOpened(formType);
            }

            return false;
        }

        public static bool IsFormOpenedOrOpening(int formType)
        {
            var uiSystem = UISystem.Get();
            if (uiSystem != null)
            {
                return uiSystem.IsFormOpenedOrOpening(formType);
            }

            return false;
        }

        public static void AddEventListener(GameObject targetUiObject,
                                            EventTriggerType eventType,
                                            UnityAction<BaseEventData> callback)
        {
            EventTrigger eventTrigger = targetUiObject.GetOrAddComponent<EventTrigger>();
            EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            trigger.AddListener(callback);
            entry.eventID = eventType;
            entry.callback = trigger;
            eventTrigger.triggers.Add(entry);
        }

        /// <summary>
        /// 添加一个可向上传递事件的EventTrigger
        /// </summary>
        /// <param name="targetUiObject"></param>
        /// <param name="eventType"></param>
        /// <param name="callback"></param>
        public static void AddPropagateEventListener(GameObject targetUiObject,
                                                     EventTriggerType eventType,
                                                     UnityAction<BaseEventData> callback)
        {
            EventTrigger eventTrigger = targetUiObject.GetOrAddComponent<PropagateEventTrigger>();
            EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            trigger.AddListener(callback);
            entry.eventID = eventType;
            entry.callback = trigger;
            eventTrigger.triggers.Add(entry);
        }

        public static void RemoveAllEventListener(GameObject targetUiObject)
        {
            EventTrigger eventTrigger = targetUiObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                return;
            }

            eventTrigger.triggers.Clear();
        }

        private static Material grayMaterial;

        public static async UniTask LoadGrayMaterialAsync()
        {
            grayMaterial = await ResourceUtility.LoadAssetAsync<Material>("Shader/UIGray.mat");
        }

        public static void SetGray(this Graphic[] graphics, bool isGray)
        {
            if (graphics == null)
            {
                return;
            }

            foreach (var graphic in graphics)
            {
                graphic.SetGray(isGray);
            }
        }

        public static void SetGray(this Graphic uiGraphic, bool isGray)
        {
            if (uiGraphic == null)
            {
                return;
            }

            uiGraphic.material = isGray ? grayMaterial : null;
        }

        public static void ShowBlackMask(float duration = .3f)
        {
            if (GetForm(BlackForm.UniqueId) is BlackForm blackForm)
            {
                blackForm.ShowBlackMask(duration);
            }
        }

        public static void EnableFullScreenUIBlock(bool isEnable)
        {
            if (GetForm(BlackForm.UniqueId) is BlackForm blackForm)
            {
                blackForm.EnableBlock(isEnable);
            }
        }

        public static async UniTask ShowBlackMaskAsync(float duration = .3f)
        {
            if (GetForm(BlackForm.UniqueId) is BlackForm blackForm)
            {
                blackForm.ShowBlackMask(duration);
            }

            await Task.Delay(TimeSpan.FromSeconds(duration));
        }

        public static void HideBlackMask(float duration = .3f)
        {
            if (GetForm(BlackForm.UniqueId) is BlackForm blackForm)
            {
                blackForm.HideBlackMask(duration);
            }
        }

        public static async UniTask HideBlackMaskAsync(float duration = .3f)
        {
            if (GetForm(BlackForm.UniqueId) is BlackForm blackForm)
            {
                blackForm.HideBlackMask(duration);
            }

            await Task.Delay(TimeSpan.FromSeconds(duration));
        }
    }
}
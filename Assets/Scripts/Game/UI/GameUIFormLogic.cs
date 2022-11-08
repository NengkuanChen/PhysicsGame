using System.Collections.Generic;
using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Game.UI
{
    public abstract class GameUIFormLogic : UIFormLogic
    {
        [SerializeField, Required, FoldoutGroup("静态UI元素列表", Order = -100)]
        private List<StaticUIElement> allStaticElements = new List<StaticUIElement>();

        public abstract int FormType { get; }

        private Canvas canvas;
        public Canvas Canvas => canvas;

        public int SortingOrder => canvas.sortingOrder;

        protected override void OnInit(object userData)
        {
            canvas = GetComponent<Canvas>();
            canvas.worldCamera = UICamera.Current.Camera;
            if (m_CachedTransform == null)
            {
                m_CachedTransform = transform;
            }

            m_UIForm = GetComponent<UIForm>();

            for (var i = 0; i < allStaticElements.Count; i++)
            {
                var element = allStaticElements[i];
                element.ownerForm = this;
                element.OnInit(userData);
            }
        }

        public void OnCustomOpen(object userData)
        {
            OnOpen(userData);
        }

        protected override void OnOpen(object userData)
        {
            m_Available = true;
            Visible = true;

            for (var i = 0; i < allStaticElements.Count; i++)
            {
                allStaticElements[i].OnOpen(userData);
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            Visible = false;
            m_Available = false;

            for (var i = 0; i < allStaticElements.Count; i++)
            {
                allStaticElements[i].OnClose();
            }
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            for (var i = 0; i < allStaticElements.Count; i++)
            {
                allStaticElements[i].OnUpdate(elapseSeconds, realElapseSeconds);
            }
        }

        protected sealed override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            var sortOrder = uiGroupDepth * 1000 + depthInUIGroup * 100;
            canvas.sortingOrder = sortOrder;
            foreach (var staticUIElement in allStaticElements)
            {
                staticUIElement.OnDepthChanged(sortOrder);
            }

            OnDepthChanged(sortOrder);
        }

        protected virtual void OnDepthChanged(int sortingOrder)
        {
        }

        public void CloseSelf()
        {
            UIUtility.CloseForm(FormType);
        }

    #if UNITY_EDITOR
        [Button(ButtonSizes.Large, Name = "刷新静态UI元素"), FoldoutGroup("静态UI元素列表", Order = -99)]
        private void RefreshFormElements()
        {
            allStaticElements.Clear();
            allStaticElements.AddRange(GetComponentsInChildren<StaticUIElement>(true));
        }

        [Button(ButtonSizes.Large, Name = "设置Canvas参数")]
        private void SetupCanvas()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
            }

            var canvasScaler = GetComponent<CanvasScaler>();
            if (canvasScaler != null)
            {
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                canvasScaler.referenceResolution = new Vector2(1080f, 1920f);
            }
        }
    #endif
    }
}

public abstract class StaticUIElement : MonoBehaviour
{
    internal UIFormLogic ownerForm;
    private Transform cacheTransform;
    public Transform CacheTransform => cacheTransform;

    public virtual void OnInit(object userData)
    {
        cacheTransform = transform;
    }

    public virtual void OnOpen(object userData)
    {
    }

    public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
    }

    public virtual void OnClose()
    {
    }

    public virtual void OnDepthChanged(int sortingOrder)
    {
    }
}
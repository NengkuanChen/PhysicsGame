using System;
using Game.GameEvent;
using Game.GameSystem;
using Game.Quality;
using Game.Utility;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game
{
    public class GameMainCamera : MonoBehaviour
    {
        private static GameMainCamera current;
        private Camera camera;
        public Camera Camera => camera;
        private UniversalAdditionalCameraData cameraData;
        public UniversalAdditionalCameraData CameraData => cameraData;
        public static GameMainCamera Current => current;

        private RenderTexture targetRenderTexture;
        public RenderTexture TargetRenderTexture => targetRenderTexture;

        private bool isRenderingEnabled;
        public bool IsRenderingEnabled => isRenderingEnabled;

        /// <summary>
        /// 如果有全屏遮挡的UI，关闭该相机
        /// </summary>
        private bool disableRenderingByUIForm;

        /// <summary>
        /// 手动关闭相机
        /// </summary>
        private bool disableRenderingManually;

        private void Awake()
        {
            camera = GetComponent<Camera>();
            if (camera == null)
            {
                throw new Exception($"主相机找不到camera {gameObject.name}");
            }

            cameraData = camera.GetUniversalAdditionalCameraData();
            camera.enabled = false;
        }

        private void OnEnable()
        {
            if (current != null)
            {
                throw new Exception($"场景中只能存在一个主相机。冲突: {current.gameObject.name} and {gameObject.name}");
            }

            current = this;
            CheckWillRendering();

            Framework.EventComponent.Subscribe(OnFormOpenedEventArgs.UniqueId, OnFormOpenOrClose);
            Framework.EventComponent.Subscribe(OnFormClosedEventArgs.UniqueId, OnFormOpenOrClose);
        }

        private void OnDisable()
        {
            Framework.EventComponent.Unsubscribe(OnFormOpenedEventArgs.UniqueId, OnFormOpenOrClose);
            Framework.EventComponent.Unsubscribe(OnFormClosedEventArgs.UniqueId, OnFormOpenOrClose);

            InternalEnableRendering(false);
            current = null;
        }

        public void RestoreRenderSize()
        {
            //相机直接做为base,ui相机做为overlay
            UICamera.Current.CameraData.renderType = CameraRenderType.Overlay;
            cameraData.renderType = CameraRenderType.Base;
            cameraData.cameraStack.Clear();
            cameraData.cameraStack.Add(UICamera.Current.Camera);
            camera.targetTexture = null;

            if (targetRenderTexture != null)
            {
                Destroy(targetRenderTexture);
                targetRenderTexture = null;
            }

            CommonUtility.CameraRectAdapte(camera);
        }

        public void ApplyRenderSize(Vector2Int renderSize)
        {
            if (Mathf.Abs(renderSize.x - QualitySystem.MaxResolution) < 2)
            {
                RestoreRenderSize();
            }
            else
            {
                //主相机生成一个低分辨率的render texture,ui相机做为base相机
                //通过render feature，将主相机的render texture blit到ui相机的render target中。
                //这样只降主相机分辨率，而ui相机以原分辨率渲染
                UICamera.Current.CameraData.renderType = CameraRenderType.Base;
                cameraData.renderType = CameraRenderType.Base;
                cameraData.cameraStack.Clear();

                if (targetRenderTexture != null)
                {
                    Destroy(targetRenderTexture);
                    targetRenderTexture = null;
                }

                targetRenderTexture =
                    new RenderTexture(renderSize.x, renderSize.y, 32, RenderTextureFormat.Default)
                    {
                        name = $"GameCameraTarget_{renderSize}"
                    };
                targetRenderTexture.Create();
                camera.targetTexture = targetRenderTexture;

                //不再需要调整相机渲染范围，因为UI相机已经调整了
                CommonUtility.RestoreCameraRect(camera);

                Log.Info($"主相机以{renderSize}分辨率渲染");
            }
        }

        private void InternalEnableRendering(bool isEnable)
        {
            if (isRenderingEnabled == isEnable)
            {
                return;
            }

            isRenderingEnabled = isEnable;
            camera.enabled = isEnable;
            if (isEnable)
            {
                var qualitySystem = QualitySystem.Get();
                ApplyRenderSize(qualitySystem.GameCameraPreferRenderingSize);
            }
            else
            {
                UICamera.Current.CameraData.renderType = CameraRenderType.Base;
                cameraData.cameraStack?.Clear();
                cameraData.renderType = CameraRenderType.Overlay;

                camera.targetTexture = null;
                if (targetRenderTexture != null)
                {
                    Destroy(targetRenderTexture);
                    targetRenderTexture = null;
                }
            }
        }

        private void OnFormOpenOrClose(object sender, GameEventArgs e)
        {
            if (e is OnFormOpenedEventArgs || e is OnFormClosedEventArgs)
            {
                CheckWillRendering();
            }
        }

        /// <summary>
        /// 检查是否开启渲染
        /// </summary>
        private void CheckWillRendering()
        {
            var uiSystem = UISystem.Get();
            if (uiSystem.HaveFullScreenCoverFormOpened && !disableRenderingByUIForm)
            {
                Log.Info("有全屏UI，隐藏主相机");
                disableRenderingByUIForm = true;
            }
            else if (!uiSystem.HaveFullScreenCoverFormOpened && disableRenderingByUIForm)
            {
                Log.Info("没有全屏UI，开启主相机");
                disableRenderingByUIForm = false;
            }

            InternalEnableRendering(!disableRenderingByUIForm && !disableRenderingManually);
        }

        public void EnableRendering(bool isEnable)
        {
            disableRenderingManually = !isEnable;
            CheckWillRendering();
        }
    }
}
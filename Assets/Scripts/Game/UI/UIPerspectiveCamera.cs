using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIPerspectiveCamera : StaticUIElement
    {
        [SerializeField, Required]
        private Canvas formCanvas;
        [SerializeField, Required]
        private Camera camera;
        public Camera Camera => camera;
        [SerializeField, Required]
        private RawImage renderTargetImage;
        public RawImage RenderTargetImage => renderTargetImage;

        private RenderTexture renderTexture;
        public RenderTexture RenderTexture => renderTexture;

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            var imageRectTransform = renderTargetImage.GetComponent<RectTransform>();
            var imageSize = imageRectTransform.rect.size;

            var uiCamera = UICamera.Current.Camera;
            var additionScale = uiCamera.rect.width;
            imageSize *= additionScale;

            renderTexture = new RenderTexture(Mathf.RoundToInt(imageSize.x),
                Mathf.RoundToInt(imageSize.y),
                32,
                RenderTextureFormat.Default) {name = $"{formCanvas.name}'s perspective camera"};
            renderTexture.Create();
            camera.targetTexture = renderTexture;
            camera.enabled = true;
            camera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;

            renderTargetImage.texture = renderTexture;

            CameraAlignToImage();
        }

        public override void OnClose()
        {
            base.OnClose();

            camera.enabled = false;
            camera.targetTexture = null;
            renderTargetImage.texture = null;
            if (renderTexture != null)
            {
                Destroy(renderTexture);
                renderTexture = null;
            }
        }

        private void CameraAlignToImage()
        {
            if (camera == null || renderTargetImage == null || formCanvas == null)
            {
                return;
            }

            var cameraPosition = camera.transform.position;
            var imagePosition = renderTargetImage.transform.position;
            cameraPosition.x = imagePosition.x;
            cameraPosition.y = imagePosition.y;
            camera.transform.position = cameraPosition;

            //计算出合适的fov
            var distanceToImage = Mathf.Abs(cameraPosition.z - imagePosition.z);
            var imageRectTransform = renderTargetImage.GetComponent<RectTransform>();
            var canvasLossyScale = formCanvas.transform.lossyScale;
            var imageHeight = imageRectTransform.rect.size.y;
            imageHeight *= canvasLossyScale.y;
            var imageVerticalHalfSize = imageHeight * .5f;
            var fov = Mathf.Atan2(imageVerticalHalfSize, distanceToImage) * Mathf.Rad2Deg * 2f;
            camera.fieldOfView = fov;
            camera.farClipPlane = distanceToImage;
        }

    #if UNITY_EDITOR

        [Button(ButtonSizes.Large, Name = "相机与image对齐")]
        private void Editor_AlignToImage()
        {
            CameraAlignToImage();
        }
    #endif
    }
}
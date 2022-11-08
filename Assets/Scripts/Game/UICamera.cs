using System;
using Game.Utility;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game
{
    public class UICamera : MonoBehaviour
    {
        private static UICamera current;
        public static UICamera Current => current;

        private Camera camera;
        public Camera Camera => camera;
        private UniversalAdditionalCameraData cameraData;
        public UniversalAdditionalCameraData CameraData => cameraData;

        private void OnEnable()
        {
            if (current != null)
            {
                throw new Exception($"只允许存在一个{nameof(UICamera)}");
            }

            current = this;

            camera = GetComponent<Camera>();
            if (camera == null)
            {
                throw new Exception($"{nameof(UICamera)}无法找到相机{gameObject.name}");
            }

            cameraData = camera.GetUniversalAdditionalCameraData();

            //宽屏两边加黑边
            // CommonUtility.CameraRectAdapte(camera);
        }

        private void OnDisable()
        {
            current = null;
            CommonUtility.RestoreCameraRect(camera);
        }

        public void EnableRendering(bool isEnable)
        {
            camera.enabled = isEnable;
        }
    }
}
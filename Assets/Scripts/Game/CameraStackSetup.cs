// using System;
// using Game.Utility;
// using UnityEngine;
// using UnityEngine.Rendering.Universal;
//
// namespace Game
// {
//     public class CameraStackSetup : MonoBehaviour
//     {
//         private static CameraStackSetup current;
//
//         private void OnEnable()
//         {
//             if (current != null)
//             {
//                 throw new Exception("场景中只能存在一个主相机");
//             }
//
//             if (UIUtility.UICameraData != null)
//             {
//                 UIUtility.UICameraData.renderType = CameraRenderType.Overlay;
//             }
//
//             var camera = GetComponent<Camera>();
//             var additionalCameraData = camera.GetUniversalAdditionalCameraData();
//             if (additionalCameraData != null)
//             {
//                 additionalCameraData.cameraStack.Clear();
//                 additionalCameraData.cameraStack.Add(UIUtility.UICamera);
//             }
//
//             current = this;
//         }
//
//         private void OnDisable()
//         {
//             var camera = GetComponent<Camera>();
//             var additionalCameraData = camera.GetUniversalAdditionalCameraData();
//             if (additionalCameraData != null)
//             {
//                 additionalCameraData.cameraStack.Clear();
//             }
//
//             if (UIUtility.UICameraData != null)
//             {
//                 UIUtility.UICameraData.renderType = CameraRenderType.Base;
//             }
//
//             current = null;
//         }
//     }
// }
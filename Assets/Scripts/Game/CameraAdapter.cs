// using Game.Utility;
// using UnityEngine;
// using UnityEngine.Rendering.Universal;
//
// namespace Game
// {
//     public class CameraAdapter : MonoBehaviour
//     {
//         private void Awake()
//         {
//             var c = GetComponent<Camera>();
//             if (c == null)
//             {
//                 return;
//             }
//
//             //宽屏两边加黑边
//             if (CommonUtility.IsWidthLayout)
//             {
//                 var width = Screen.height * (1080f / 1920f) / Screen.width;
//                 c.rect = new Rect((1f - width) * .5f, 0f, width, 1f);
//             }
//
//         }
//     }
// }
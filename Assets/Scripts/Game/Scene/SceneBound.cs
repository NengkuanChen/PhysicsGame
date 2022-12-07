using Game.UI.Form;
using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scene
{
    public class SceneBound: UniqueSceneElement<SceneBound>
    {
        [SerializeField, LabelText("Top Bound")]
        private Transform topBound;
        
        [SerializeField, LabelText("Bottom Bound")]
        private Transform bottomBound;
        
        [SerializeField, LabelText("Left Bound")]
        private Transform leftBound;
        
        [SerializeField, LabelText("Right Bound")]
        private Transform rightBound;
        

        public void SetBound()
        {
            var ratio = (float)Screen.width/Screen.height;
            ratio = Mathf.Min(ratio, 1080f / 1920f);
            // topBound.position =
            //     GameMainCamera.Current.Camera.ScreenToWorldPoint(boundForm.TopBound.localPosition + offSet) +
            //     Vector3.up * .5f;
            
            // leftBound.position = GameMainCamera.Current.Camera.ScreenToWorldPoint(boundForm.LeftBound.localPosition + offSet) +
            //                      Vector3.left * 1.5f;
            // rightBound.position = GameMainCamera.Current.Camera.ScreenToWorldPoint(boundForm.RightBound.localPosition + offSet) +
            //                       Vector3.right * 1.5f;
            leftBound.position = new Vector3(-10.48f * ratio, 0, 0) - Vector3.right * 0.25f;
            rightBound.position = new Vector3(10.48f * ratio, 0, 0) + Vector3.right * 0.25f;
        }
    }
}
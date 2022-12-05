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
            var boundForm = UIUtility.GetForm<BoundForm>(BoundForm.UniqueId);
            Log.Info($"{boundForm.Canvas.pixelRect.height}, {boundForm.Canvas.pixelRect.width}");
            Log.Info($"{boundForm.TopBound.localPosition}");
            var offSet = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            topBound.position =
                GameMainCamera.Current.Camera.ScreenToWorldPoint(boundForm.TopBound.localPosition + offSet) +
                Vector3.up * .5f;
            bottomBound.position = GameMainCamera.Current.Camera.ScreenToWorldPoint(boundForm.BottomBound.localPosition + offSet) +
                                   Vector3.down * .5f;
            leftBound.position = GameMainCamera.Current.Camera.ScreenToWorldPoint(boundForm.LeftBound.localPosition + offSet) +
                                 Vector3.left * 1.5f;
            rightBound.position = GameMainCamera.Current.Camera.ScreenToWorldPoint(boundForm.RightBound.localPosition + offSet) +
                                  Vector3.right * 1.5f;
            
        }
    }
}
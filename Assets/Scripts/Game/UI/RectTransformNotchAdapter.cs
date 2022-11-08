using Game.Utility;
using UnityEngine;

namespace Game
{
    public class RectTransformNotchAdapter : MonoBehaviour
    {
        private void Awake()
        {
            if (CommonUtility.ScreenHaveNotch)
            {
                var rectTransform = GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    var safeArea = Screen.safeArea;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;

                    var heightScale = rectTransform.rect.height / Screen.height;
                    rectTransform.offsetMin = new Vector2(0f, safeArea.y * heightScale);
                    rectTransform.offsetMax =
                        new Vector2(0f, -(Screen.height - safeArea.height - safeArea.y) * heightScale);
                }
            }
        }
    }
}
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 支持多次点击按钮(双击/三击等)
    /// </summary>
    public class MultiClickInvoker : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField, LabelText("点击次数")]
        private int validClickTimes = 2;

        [SerializeField, LabelText("点击间隔")]
        private float validClickInterval = 1f;

        private int clickCount;
        private float lastClickTime;

        private Button.ButtonClickedEvent onClick = new Button.ButtonClickedEvent();
        public Button.ButtonClickedEvent OnClick => onClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            var currentTime = Time.unscaledTime;
            if (clickCount == 0)
            {
                clickCount++;
            }
            else
            {
                var clickInterval = currentTime - lastClickTime;
                if (clickInterval <= validClickInterval)
                {
                    clickCount++;
                    // Log.Info($"click count {clickCount}");
                    if (clickCount == validClickTimes)
                    {
                        clickCount = 0;
                        onClick?.Invoke();
                    }
                }
                else
                {
                    clickCount = 0;
                }
            }

            lastClickTime = currentTime;
        }
    }
}
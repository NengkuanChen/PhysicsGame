using Game.Utility;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIGrayHandler : MonoBehaviour
    {
        [SerializeField, Required]
        private Image[] images;
        public Image[] Images => images;
        [SerializeField, Required]
        private TextMeshProUGUI[] tmpTexts;
        public TextMeshProUGUI[] TMPTexts => tmpTexts;

        private Color[] originTextColors;

        private void Awake()
        {
            if (tmpTexts != null && tmpTexts.Length > 0)
            {
                originTextColors = new Color[tmpTexts.Length];
                for (var i = 0; i < tmpTexts.Length; i++)
                {
                    originTextColors[i] = tmpTexts[i].color;
                }
            }
        }

        public void SetGray(bool isGray)
        {
            images.SetGray(isGray);

            if (isGray)
            {
                //处理字体的颜色变化,算法保持和shader一致
                if (tmpTexts != null)
                {
                    for (var i = 0; i < tmpTexts.Length; i++)
                    {
                        var text = tmpTexts[i];
                        var originColor = originTextColors[i];
                        var grayScale = originColor.r * .299f + originColor.g * .587f + originColor.b * .114f;
                        text.color = new Color(grayScale, grayScale, grayScale, originColor.a);
                    }
                }
            }
            else
            {
                if (tmpTexts != null)
                {
                    for (var i = 0; i < tmpTexts.Length; i++)
                    {
                        var text = tmpTexts[i];
                        var originColor = originTextColors[i];
                        text.color = originColor;
                    }
                }
            }
        }

    #if UNITY_EDITOR

        [Button(ButtonSizes.Large)]
        private void Setup()
        {
            images = GetComponentsInChildren<Image>(includeInactive:true);
            tmpTexts = GetComponentsInChildren<TextMeshProUGUI>();
        }
    #endif
    }
}
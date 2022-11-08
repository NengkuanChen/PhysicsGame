using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField, Required]
        private Button button;
        [SerializeField, Required]
        private Image toggleIconImage;
        [SerializeField, Required]
        private Sprite onSprite;
        [SerializeField, Required]
        private Sprite offSprite;

        public Image ButtonImage => button.image;
        public Button.ButtonClickedEvent OnClick => button.onClick;
        
        public void On()
        {
            toggleIconImage.sprite = onSprite;
        }

        public void Off()
        {
            toggleIconImage.sprite = offSprite;
        }
    }
}
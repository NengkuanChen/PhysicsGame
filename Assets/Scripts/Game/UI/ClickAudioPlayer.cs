using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class ClickAudioPlayer : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField, Required]
        private int soundId;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            // SoundUtility.PlaySound(soundId);
        }
    }
}
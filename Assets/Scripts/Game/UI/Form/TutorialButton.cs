using Game.GameEvent;
using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Form
{
    public class TutorialButton: StaticUIElement
    {
        [SerializeField] 
        private Button tutorialButton;
        
        

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            tutorialButton.onClick.AddListener(() =>
            {
                // Framework.EventComponent.Fire(this, OnGamePauseEventArgs.Create(true));
                if (UIUtility.GetForm(TutorialForm.UniqueId) != null)
                {
                    return;
                }
                UIUtility.OpenForm(TutorialForm.UniqueId);
                tutorialButton.interactable = false;
            });
        }
        
    }
}
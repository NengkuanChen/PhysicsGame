using GameFramework.UI;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game.UI
{
    public class CustomUIFormHelper : DefaultUIFormHelper
    {
        public override IUIForm CreateUIForm(object uiFormInstance, IUIGroup uiGroup, object userData)
        {
            var gameObject = uiFormInstance as GameObject;
            if (gameObject == null)
            {
                Log.Error("UI form instance is invalid.");
                return null;
            }

            var transform = gameObject.transform;
            transform.SetParent(((MonoBehaviour)uiGroup.Helper).transform);
            transform.localScale = Vector3.one;

            return gameObject.GetOrAddComponent<GameUIForm>();
        }
    }
}
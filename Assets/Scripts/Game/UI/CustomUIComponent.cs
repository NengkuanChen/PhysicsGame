using UnityGameFramework.Runtime;
#if UNITY_EDITOR
using UnityEditor;
using UnityGameFramework.Editor;
#endif

namespace Game.UI
{
    public class CustomUIComponent : UIComponent
    {
        protected override void OnOpenUIFormSuccess(object sender, GameFramework.UI.OpenUIFormSuccessEventArgs e)
        {
            m_EventComponent.FireNow(this, OpenUIFormSuccessEventArgs.Create(e));
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomUIComponent))]
    public class CustomUIComponentInspector : UIComponentInspector
    {
    }
#endif
}
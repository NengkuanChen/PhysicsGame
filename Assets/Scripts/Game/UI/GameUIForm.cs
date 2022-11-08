using System;
using UnityGameFramework.Runtime;

namespace Game.UI
{
    public class GameUIForm : UIForm
    {
        public override void OnOpen(object userData)
        {
            // 
            // try
            // {
            //     if (userData is OpenFormData openFormData)
            //     {
            //         var uiFormLogic = m_UIFormLogic as GameUIFormLogic;
            //         uiFormLogic.OnCustomOpen(openFormData.openUserData);
            //     }
            // }
            // catch (Exception exception)
            // {
            //     Log.Error($"UI form '[{m_SerialId}]{UIFormAssetName}' OnOpen with exception '{exception}'.");
            // }
        }
    }
}
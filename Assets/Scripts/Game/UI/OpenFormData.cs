using System;
using GameFramework;

namespace Game.UI
{
    public class OpenFormData : IReference
    {
        public object openUserData;
        public int formType;
        public Action<GameUIFormLogic> onComplete;
        public Action onFailure;

        public void Clear()
        {
            formType = -1;
            openUserData = null;
            onComplete = null;
            onFailure = null;
        }
    }
}
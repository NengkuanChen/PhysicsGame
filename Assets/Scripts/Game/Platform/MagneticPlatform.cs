using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.PlatForm
{
    public class MagneticPlatform: PlatformEntity
    {
        [SerializeField, Required, LabelText("Magnetic Range")] 
        private MagneticTrigger triggerCollider;

        [SerializeField, Required, LabelText("Magnetic Platform Setting")]
        private MagneticPlatformSetting setting;

        
    }
}
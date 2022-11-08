using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Editor.Build
{
    [CreateAssetMenu(fileName = "PackageInfoConfiguration(IOS)",
        menuName = "Build/PackageInfoConfiguration(IOS)",
        order = 1)]
    public class IOSPackageInfoConfiguration : PackageInfoConfiguration
    {
        [BoxGroup("IOS"), SerializeField, LabelText("APP ID")]
        private int iosAppId;
        public int IOSAppId => iosAppId;
    }
}
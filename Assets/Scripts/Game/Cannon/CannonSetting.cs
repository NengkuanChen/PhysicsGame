using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Cannon
{
    [CreateAssetMenu(fileName = "CannonSetting", menuName = "CannonSetting", order = 0)]
    public class CannonSetting : ScriptableObject
    {
        [SerializeField, LabelText("Cannon Launch Speed")]
        private float launchSpeed;

        public float LaunchSpeed => launchSpeed;

        [SerializeField, LabelText("Cannon Reloading Time")]
        private float reloadingTime;

        public float ReloadingTime => reloadingTime;

    }
}
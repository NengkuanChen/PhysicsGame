using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
namespace Game.Common
{
    public class MeshReadableFlag : MonoBehaviour
    {
        [SerializeField, Required,InfoBox("需要修改mesh数据但是没有附加mesh collide组件的mesh filter.")]
        private MeshFilter[] meshFilters;
        public MeshFilter[] MeshFilters => meshFilters;
    }
}
#endif
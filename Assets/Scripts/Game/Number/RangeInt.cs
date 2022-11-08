using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;

#endif

namespace Game.Number
{
    [Serializable]
    public struct RangeInt
    {
        [SerializeField]
        public int min;
        [SerializeField]
        public int max;

        public RangeInt(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RangeIntAttribute : Attribute
    {
        public int min;
        public int max;

        public RangeIntAttribute(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }

#if UNITY_EDITOR

    public class RangeIntAttributeDrawer : OdinAttributeDrawer<RangeIntAttribute, RangeInt>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            label.text = $"{label.text},({value.min} - {value.max})";
            EditorGUILayout.LabelField(label);
            var currentMin = (float) value.min;
            var currentMax = (float) value.max;
            EditorGUILayout.MinMaxSlider(ref currentMin, ref currentMax, 0, 10);
            value.min = Mathf.RoundToInt(currentMin);
            value.max = Mathf.RoundToInt(currentMax);
            ValueEntry.SmartValue = value;
        }
    }
#endif
}
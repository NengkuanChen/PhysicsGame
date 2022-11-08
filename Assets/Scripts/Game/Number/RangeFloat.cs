using System;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Number
{
    [Serializable]
    public struct RangeFloat
    {
        [SerializeField]
        public float min;
        [SerializeField]
        public float max;

        public RangeFloat(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float RandomValue()
        {
            return Random.Range(min, max);
        }

        public bool Contain(float f)
        {
            return f >= min && f <= max;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RangeFloatAttribute : Attribute
    {
        public float min;
        public float max;

        public RangeFloatAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

#if UNITY_EDITOR

    public class RangeFloatAttributeDrawer : OdinAttributeDrawer<RangeFloatAttribute, RangeFloat>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            EditorGUILayout.LabelField(label);
            var currentMin = value.min;
            var currentMax = value.max;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.MinMaxSlider(ref currentMin, ref currentMax, Attribute.min, Attribute.max);
            currentMin = EditorGUILayout.FloatField(currentMin, GUILayout.Width(50));
            currentMin = Mathf.Clamp(currentMin, Attribute.min, Attribute.max);
            currentMax = EditorGUILayout.FloatField(currentMax, GUILayout.Width(50));
            currentMax = Mathf.Clamp(currentMax, currentMin, Attribute.max);
            EditorGUILayout.EndHorizontal();
            value.min = currentMin;
            value.max = currentMax;
            ValueEntry.SmartValue = value;
        }
    }
#endif
}
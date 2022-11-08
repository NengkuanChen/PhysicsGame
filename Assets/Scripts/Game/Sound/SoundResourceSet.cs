using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Sound
{
    [Serializable]
    public class SoundProperty
    {
    #if UNITY_EDITOR
        [SerializeField]
        private string description = "----------新音频属性----------";
    #endif
        [SerializeField]
        public int index;
        public int Index => index;

        [SerializeField]
        public AudioClip clip;
        public AudioClip Clip => clip;
        [SerializeField,Range(0f, 1f)]
        public float volume = 1f;
        public float Volume => volume;

        [SerializeField,LabelText("淡入时间")]
        public float fadeInTime = 0f;
        public float FadeInTime => fadeInTime;
        [SerializeField,LabelText("淡出时间")]
        public float fadeOutTime = 0f;
        public float FadeOutTime => fadeOutTime;

        [SerializeField,LabelText("允许在多个音频播放"), OnValueChanged("OnCanDuplicateOrLoopChanged")]
        public bool canDuplicate;
        public bool CanDuplicate => canDuplicate;

        [SerializeField,LabelText("循环播放"), OnValueChanged("OnCanDuplicateOrLoopChanged")]
        public bool isLoop = false;
        public bool IsLoop => isLoop;

    #if UNITY_EDITOR
        private void OnCanDuplicateOrLoopChanged()
        {
            if (isLoop && canDuplicate)
            {
                //不允许即是循环 又能多重播放
                canDuplicate = false;
            }
        }
    #endif
    }

    [CreateAssetMenu(fileName = "SoundSet", menuName = "Resource/SoundSet", order = 0)]
    public class SoundResourceSet : ScriptableObject
    {
        [SerializeField,
         ListDrawerSettings(DraggableItems = false,
             ShowIndexLabels = true,
             Expanded = true,
             CustomAddFunction = "AddNewSoundPropertyItem")]
        private List<SoundProperty> soundProperties;

        private readonly Dictionary<int, SoundProperty> soundPropertyMap = new Dictionary<int, SoundProperty>();

        private void OnEnable()
        {
            if (soundProperties == null)
            {
                return;
            }

            foreach (var soundProperty in soundProperties)
            {
                if (soundProperty == null)
                {
                    Debug.LogError("有音效配置丢失了引用");
                    return;
                }

                soundPropertyMap.Add(soundProperty.index, soundProperty);
            }
        }

        public SoundProperty GetAudio(int id)
        {
            if (soundPropertyMap.TryGetValue(id, out var property))
            {
                return property;
            }

            Log.Error($"无法获取到音频文件. id: {id}");
            return null;
        }

    #if UNITY_EDITOR
        private void AddNewSoundPropertyItem()
        {
            if (soundProperties.Count > 0)
            {
                var lastProperty = soundProperties.Last();
                var newIndex = lastProperty.index + 1;
                soundProperties.Add(new SoundProperty
                {
                    index = newIndex,
                    canDuplicate = true,
                    isLoop = false,
                });
            }
            else
            {
                soundProperties.Add(new SoundProperty
                {
                    index = 0,
                    canDuplicate = true,
                    isLoop = false,
                });
            }
        }
    #endif
    }
}
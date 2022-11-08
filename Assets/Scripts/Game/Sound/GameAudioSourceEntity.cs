using DG.Tweening;
using UnityEngine;

namespace Game.Sound
{
    public class GameAudioSourceEntity
    {
        private AudioSource audioSource;
        public AudioSource AudioSource => audioSource;

        private SoundProperty soundProperty;
        public SoundProperty SoundProperty
        {
            get => soundProperty;
            set
            {
                soundProperty = value;
                audioSource.clip = soundProperty.clip;
                audioSource.volume = soundProperty.volume;
                audioSource.loop = soundProperty.isLoop;
            }
        }

        public bool IsPlaying => audioSource.isPlaying;
        public AudioClip Clip => audioSource.clip;

        public float Volume
        {
            get
            {
                if (overrideVolume != null)
                {
                    return overrideVolume.Value;
                }

                return soundProperty.volume;
            }
        }
        float? overrideVolume;

        public GameAudioSourceEntity(AudioSource audioSource)
        {
            this.audioSource = audioSource;
        }

        public void Play()
        {
            DOTween.Kill(audioSource);
            if (soundProperty.fadeInTime > 0f)
            {
                audioSource.volume = 0f;
                audioSource.DOFade(Volume, soundProperty.fadeInTime);
            }
            else
            {
                audioSource.volume = Volume;
            }

            audioSource.Play();
        }

        public void Stop()
        {
            if (audioSource == null)
            {
                return;
            }
            
            if (!audioSource.isPlaying)
            {
                return;
            }

            DOTween.Kill(audioSource);
            if (soundProperty.fadeOutTime > 0f)
            {
                audioSource.DOFade(0f, soundProperty.fadeOutTime).onComplete = StopAudioSourceInternal;
            }
            else
            {
                audioSource.Stop();
            }
        }

        private void StopAudioSourceInternal()
        {
            audioSource.Stop();
        }

        public void OverrideVolume(float? volume)
        {
            overrideVolume = volume;
            audioSource.volume = Volume;
        }
    }
}
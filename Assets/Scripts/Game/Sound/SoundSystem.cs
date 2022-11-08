using System.Collections.Generic;
using Game.GameEvent;
using Game.GameSystem;
using GameFramework.Event;
using UnityEngine;

namespace Game.Sound
{
    public class SoundSystem : SystemBase
    {
        public static SoundSystem Get()
        {
            return SystemEntry.GetSystem(id) as SoundSystem;
        }

        private static int id = UniqueIdGenerator.GetUniqueId();
        internal override int ID => id;

        private GameObject audioSourceRootGameObject;
        private int spawnedAudioSourceCount;

        private List<GameAudioSourceEntity> freeGameAudioSourceEntities = new List<GameAudioSourceEntity>();
        private List<GameAudioSourceEntity> playingGameAudioSourcesEntities = new List<GameAudioSourceEntity>();
        private HashSet<GameAudioSourceEntity> obtainedGameAudioSourcesEntities = new HashSet<GameAudioSourceEntity>();

        internal override void OnEnable()
        {
            base.OnEnable();
            audioSourceRootGameObject = new GameObject("Audio Source - 0");

            Framework.EventComponent.Subscribe(OnAudioStatusChangedEventArgs.UniqueId, OnSoundStatusChanged);
        }

        internal override void OnDisable()
        {
            base.OnDisable();
            
            Framework.EventComponent.Unsubscribe(OnAudioStatusChangedEventArgs.UniqueId, OnSoundStatusChanged);

        }

        internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            for (var i = playingGameAudioSourcesEntities.Count - 1; i >= 0; i--)
            {
                var audioSourceInfo = playingGameAudioSourcesEntities[i];
                if (!audioSourceInfo.IsPlaying)
                {
                    audioSourceInfo.AudioSource.clip = null;
                    playingGameAudioSourcesEntities.RemoveAt(i);
                    freeGameAudioSourceEntities.Add(audioSourceInfo);
                }
            }
        }

        public void Play(SoundProperty soundProperty)
        {
            if (soundProperty == null)
            {
                return;
            }

            if (!soundProperty.canDuplicate)
            {
                for (var i = 0; i < playingGameAudioSourcesEntities.Count; i++)
                {
                    var audioSourceInfo = playingGameAudioSourcesEntities[i];
                    if (audioSourceInfo.Clip == soundProperty.clip)
                    {
                        return;
                    }
                }
            }

            var audioSource = GetFreeAudioSource(soundProperty);
            // Log.Info($"play sound {soundProperty.index}");
            audioSource.Play();
            playingGameAudioSourcesEntities.Add(audioSource);
        }

        public void StopPlay(SoundProperty soundProperty)
        {
            if (soundProperty == null)
            {
                return;
            }

            foreach (var playingAudioSourceEntity in playingGameAudioSourcesEntities)
            {
                if (playingAudioSourceEntity.SoundProperty == soundProperty)
                {
                    playingAudioSourceEntity.Stop();
                }
            }
        }

        public GameAudioSourceEntity ObtainAudioSource(SoundProperty soundProperty)
        {
            var audioSource = GetFreeAudioSource(soundProperty);
            obtainedGameAudioSourcesEntities.Add(audioSource);
            return audioSource;
        }

        public void ReleaseObtainedAudioSource(GameAudioSourceEntity audioSourceEntity)
        {
            if (audioSourceEntity == null)
            {
                return;
            }

            audioSourceEntity.Stop();
            freeGameAudioSourceEntities.Add(audioSourceEntity);
            obtainedGameAudioSourcesEntities.Remove(audioSourceEntity);
        }

        private GameAudioSourceEntity GetFreeAudioSource(SoundProperty soundProperty)
        {
            GameAudioSourceEntity result;
            if (freeGameAudioSourceEntities.Count > 0)
            {
                result = freeGameAudioSourceEntities[freeGameAudioSourceEntities.Count - 1];
                freeGameAudioSourceEntities.RemoveAt(freeGameAudioSourceEntities.Count - 1);
                result.SoundProperty = soundProperty;
            }
            else
            {
                var newAudioSource = audioSourceRootGameObject.AddComponent<AudioSource>();
                result = new GameAudioSourceEntity(newAudioSource) {SoundProperty = soundProperty};
                spawnedAudioSourceCount++;
                audioSourceRootGameObject.name = $"Audio Source - {spawnedAudioSourceCount}";
            }

            var audioSource = result.AudioSource;
            audioSource.spatialBlend = 0f;
            audioSource.playOnAwake = false;
            audioSource.Stop();

            return result;
        }

        private void OnSoundStatusChanged(object sender, GameEventArgs e)
        {
            if (e is OnAudioStatusChangedEventArgs args)
            {
                AudioListener.volume = args.IsEnable ? 1f : 0f;
            }
        }
    }
}
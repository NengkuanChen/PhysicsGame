using Cysharp.Threading.Tasks;
using Game.Sound;

namespace Game.Utility
{
    public static class SoundUtility
    {
        public static SoundResourceSet SoundResourceSet { get; private set; }

        public static async UniTask InitialLoadAsync()
        {
            // SoundResourceSet = await ResourceUtility.LoadAssetAsync<SoundResourceSet>("Sound/SoundSet.asset");
        }

        public static void PlaySound(int soundId)
        {
            if (SoundResourceSet != null)
            {
                var soundProperty = SoundResourceSet.GetAudio(soundId);
                if (soundProperty != null)
                {
                    var soundSystem = SoundSystem.Get();
                    soundSystem.Play(soundProperty);
                }
            }
        }
        
        public static void StopSound(int soundId)
        {
            if (SoundResourceSet != null)
            {
                var soundProperty = SoundResourceSet.GetAudio(soundId);
                if (soundProperty != null)
                {
                    var soundSystem = SoundSystem.Get();
                    soundSystem?.StopPlay(soundProperty);
                }
            }
        }

        public static void PlayButtonClickAudio()
        {
            PlaySound(0);
        }

        public static GameAudioSourceEntity ObtainSetupAudioSource(int soundId)
        {
            if (SoundResourceSet != null)
            {
                var soundProperty = SoundResourceSet.GetAudio(soundId);
                if (soundProperty != null)
                {
                    var soundSystem = SoundSystem.Get();
                    return soundSystem.ObtainAudioSource(soundProperty);
                }
            }

            return null;
        }

        public static void ReleaseObtainedAudioSource(GameAudioSourceEntity gameAudioSource)
        {
            if (gameAudioSource == null)
            {
                return;
            }

            SoundSystem.Get()?.ReleaseObtainedAudioSource(gameAudioSource);
        }
    }
}
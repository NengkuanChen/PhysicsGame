using Cysharp.Threading.Tasks;
using Game.Entity;
using Game.GameCamera;
using Game.Utility;

namespace Game.GameSystem
{
    public class GameCameraSystem: SystemBase
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();

        internal override int ID => UniqueId;

        private CameraEntity gameCameraEntity;

        public CameraEntity GameCameraEntity => gameCameraEntity;

        public static GameCameraSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as GameCameraSystem;
        }

        public async UniTask<CameraEntity> LoadCameraAsync()
        {
            if (gameCameraEntity != null)
            {
                gameCameraEntity.Hide();
            }

            gameCameraEntity =
                await EntityUtility.ShowEntityAsync<CameraEntity>("Camera/GameCamera", EntityGroupName.Camera);
            return gameCameraEntity;
        }
    }
}
using Cysharp.Threading.Tasks;
using Game.Cannon;
using Game.Entity;
using Game.Utility;

namespace Game.GameSystem
{
    public class CannonSystem: SystemBase
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();

        internal override int ID => UniqueId;

        public static CannonSystem Get()
        {
            return SystemEntry.GetSystem(UniqueId) as CannonSystem;
        }


        public CannonEntity currentCannon;
        public CannonEntity CurrentCannon => currentCannon;


        public async UniTask<CannonEntity> LoadCannonAsync()
        {
            if (currentCannon != null)
            {
                currentCannon.Hide();
            }

            currentCannon = await EntityUtility.ShowEntityAsync<CannonEntity>("Cannon/Cannon", EntityGroupName.Cannon);
            return currentCannon;
        }
    }
}
using DG.Tweening;
using Game.Entity;
using Game.Utility;

namespace Game.Ball
{
    public class BallSwitchEffectComponent: SpecificEntityComponentBase<BallSwitchEntity>
    {
        public static readonly int UniqueId = UniqueIdGenerator.GetUniqueId();
        public override int ID => UniqueId;

        private BallType ballType;
        

        public BallSwitchEffectComponent(BallType ballType)
        {
            this.ballType = ballType;
        }

        public override void OnComponentAttach()
        {
            base.OnComponentAttach();
            OwnerEntityType.SwitchPlane.DOKill();
            OwnerEntityType.SwitchPlane.DOLocalMoveX(
                ballType == BallType.IronBall ? OwnerEntityType.IronBallX : OwnerEntityType.WoodBallX,
                OwnerEntityType.SwitchTime).OnComplete(() =>
            {
                OwnerEntity.RemoveComponent(BallSwitchFollowComponent.UniqueId);
                RemoveSelf();
            });
        }

        public override void OnComponentDetach()
        {
            base.OnComponentDetach();
            OwnerEntityType.SwitchPlane.DOKill();
        }
    }
}
using Blobcreate.ProjectileToolkit;
using Game.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Cannon
{
    public class CannonEntity: GameEntityLogic
    {
        [SerializeField, LabelText("CannonSetting"), Required]
        private CannonSetting setting;

        public CannonSetting Setting => setting;

        [SerializeField, LabelText("Cannon Barrel"), Required]
        private Transform cannonBarrel;

        public Transform CannonBarrel => cannonBarrel;
        
        [SerializeField, LabelText("Projectile Predictor"), Required] 
        private TrajectoryPredictor preditor;

        public TrajectoryPredictor Predictor => preditor;
        
        
    }
}
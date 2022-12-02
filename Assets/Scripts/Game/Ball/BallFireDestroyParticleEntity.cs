using Cysharp.Threading.Tasks;
using Game.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Ball
{
    public class BallFireDestroyParticleEntity: GameEntityLogic
    {
        [SerializeField, LabelText("Fire Particle")] 
        private ParticleSystem fireParticle;
        public ParticleSystem FireParticle => fireParticle;


        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
        }
        
    }
}
using System;
using UnityEngine;

namespace Game.PlatForm
{
    public class SeesawPlatform: PlatformEntity
    {
        [SerializeField]
        private Rigidbody platformRigidbody;

        public override void OnPlatformShow()
        {
            base.OnPlatformShow();
            transform.rotation = Quaternion.identity;
            platformRigidbody.angularVelocity = Vector3.zero;
        }
    }
}
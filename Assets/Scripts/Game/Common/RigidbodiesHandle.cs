using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Common
{
    public class RigidbodiesHandle
    {
        private List<Rigidbody> rigidbodies = new List<Rigidbody>();
        public List<Rigidbody> Rigidbodies => rigidbodies;

        private List<Vector3> originPositions = new List<Vector3>();
        private List<Quaternion> originRotations = new List<Quaternion>();
        private List<Vector3> originLocalScales = new List<Vector3>();
        private List<Transform> originParents = new List<Transform>();

        public void RecordRigidbodies(IList<Rigidbody> content)
        {
            Assert.IsNotNull(content);

            rigidbodies.Clear();
            rigidbodies.AddRange(content);

            originPositions.Clear();
            originRotations.Clear();
            originParents.Clear();
            foreach (var rigidbody in rigidbodies)
            {
                var transform = rigidbody.transform;
                originPositions.Add(transform.localPosition);
                originRotations.Add(transform.localRotation);
                originLocalScales.Add(transform.localScale);
                originParents.Add(transform.parent);
            }
        }

        public void ResetRigidbodies(Action<Rigidbody> resetCallback = null)
        {
            if (rigidbodies != null)
            {
                for (var i = 0; i < rigidbodies.Count; i++)
                {
                    var rigidbody = rigidbodies[i];
                    if (rigidbody == null)
                    {
                        continue;
                    }

                    var rigidbodyTransform = rigidbody.transform;
                    if (rigidbodyTransform == null)
                    {
                        continue;
                    }

                    rigidbodyTransform.SetParent(originParents[i]);
                    rigidbodyTransform.localPosition = originPositions[i];
                    rigidbodyTransform.localRotation = originRotations[i];
                    rigidbodyTransform.localScale = originLocalScales[i];

                    rigidbody.velocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                    rigidbody.Sleep();
                    resetCallback?.Invoke(rigidbody);
                }
            }
        }
    }
}
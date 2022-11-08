using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class TransformRotate : MonoBehaviour
    {
        [SerializeField]
        private Vector3 rotateAxis = Vector3.forward;
        [SerializeField]
        private float rotateSpeed = 180f;
        [SerializeField]
        private bool randomInitialRotation;

        private void OnEnable()
        {
            if (randomInitialRotation)
            {
                var randomAngle = Random.Range(0f, 360f);
                transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);
            }
        }

        private void Update()
        {
            transform.Rotate(rotateAxis, rotateSpeed * Time.deltaTime);
        }
    }
}
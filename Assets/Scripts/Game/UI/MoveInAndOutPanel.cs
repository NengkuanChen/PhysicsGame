using System;
using System.Collections;
using DG.Tweening;
using Game.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI
{
    public class MoveInAndOutPanel : MonoBehaviour
    {
        [SerializeField, Required]
        private RectTransform rootTransform;
        [SerializeField, Required]
        private RectTransform insidePoint;
        [SerializeField, Required]
        private RectTransform outsidePoint;
        [SerializeField, Min(0f)]
        private float moveInTime = 1f;
        [SerializeField]
        private Ease moveInEaseType = Ease.OutCubic;
        [SerializeField, Min(0f)]
        private float moveOutTime = 1f;
        [SerializeField]
        private Ease moveOutEaseType = Ease.InCubic;

        private bool isMoving;
        public bool IsMoving => isMoving;

        private void OnDisable()
        {
            isMoving = false;
        }

        public void MoveIn(Action onComplete = null)
        {
            StopAllCoroutines();
            StartCoroutine(MoveInCoroutine(onComplete));
        }

        public void MoveOut(Action onComplete = null)
        {
            StopAllCoroutines();
            StartCoroutine(MoveOutCoroutine(onComplete));
        }

        public IEnumerator MoveInCoroutine(Action onComplete = null)
        {
            isMoving = true;
            DOTween.Kill(rootTransform);
            var parent = rootTransform.parent;
            var outsideLocalPosition = parent.InverseTransformPoint(outsidePoint.position);
            var insideLocalPosition = parent.InverseTransformPoint(insidePoint.position);
            rootTransform.localPosition = outsideLocalPosition;
            yield return rootTransform.DOLocalMove(insideLocalPosition, moveInTime)
                                      .SetEase(moveInEaseType)
                                      .WaitForCompletion();
            onComplete?.Invoke();
            isMoving = false;
        }

        public IEnumerator MoveOutCoroutine(Action onComplete = null)
        {
            isMoving = true;
            DOTween.Kill(rootTransform);
            var parent = rootTransform.parent;
            var outsideLocalPosition = parent.InverseTransformPoint(outsidePoint.position);
            yield return rootTransform.DOAnchorPos(outsideLocalPosition, moveOutTime)
                                      .SetEase(moveOutEaseType)
                                      .WaitForCompletion();
            onComplete?.Invoke();
            isMoving = false;
        }

    #if UNITY_EDITOR
        [Button(ButtonSizes.Large, Name = "与Inside位置对齐")]
        private void AlignToInsidePoint()
        {
            if (rootTransform == null || insidePoint == null)
            {
                return;
            }

            rootTransform.position = insidePoint.position;
        }

        [Button(ButtonSizes.Large, Name = "与Outside位置对齐")]
        private void AlignToOutsidePoint()
        {
            if (rootTransform == null || outsidePoint == null)
            {
                return;
            }

            rootTransform.position = outsidePoint.position;
        }

        private void OnDrawGizmosSelected()
        {
            if (rootTransform == null || insidePoint == null || outsidePoint == null)
            {
                return;
            }

            DebugExtension.DrawArrow(rootTransform.position,
                (insidePoint.position - rootTransform.position),
                Color.green);
            DebugExtension.DrawArrow(rootTransform.position,
                (outsidePoint.position - rootTransform.position),
                Color.red);
        }
    #endif
    }
}
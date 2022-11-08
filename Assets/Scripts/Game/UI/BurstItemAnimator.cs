using System;
using System.Collections;
using DG.Tweening;
using Game;
using Game.Utility;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game.UI
{
    /// <summary>
    /// 仅用于UI
    /// </summary>
    public class BurstItemAnimator
    {
        private RectTransform itemTemplate;
        private SimpleObjectPool<RectTransform> itemPool;
        private IEnumerator cor;
        private Action onAnimationComplete;

        private bool isAnimationComplete = true;
        public bool IsAnimationComplete => isAnimationComplete;

        public BurstItemAnimator(RectTransform itemTemplate)
        {
            this.itemTemplate = itemTemplate;
            itemPool = new SimpleObjectPool<RectTransform>(() => Object.Instantiate(itemTemplate, itemTemplate.parent),
                transform => { transform.gameObject.SetActive(true); },
                transform =>
                {
                    DOTween.Kill(transform);
                    transform.gameObject.SetActive(false);
                },
                transform => { Object.Destroy(transform.gameObject); });
            itemTemplate.gameObject.SetActive(false);
        }

        public void PlayAnimation(int spawnCount,
                                  Vector3 startPosition,
                                  float randomStartRange,
                                  float startControlPointRange,
                                  Vector3 endPosition,
                                  float randomEndRange,
                                  float endControlPointRange,
                                  float eachBurstAnimTime,
                                  float burstInterval,
                                  Action onComplete = null,
                                  Action onEachItemMoveComplete = null)
        {
            if (!isAnimationComplete)
            {
                Complete();
            }

            cor = PlayAnimationCoroutine(spawnCount,
                startPosition,
                randomStartRange,
                startControlPointRange,
                endPosition,
                randomEndRange,
                endControlPointRange,
                eachBurstAnimTime,
                burstInterval,
                onComplete,
                onEachItemMoveComplete).StartCoroutine();
        }

        public IEnumerator PlayAnimationCoroutine(int spawnCount,
                                                  Vector3 startPosition,
                                                  float randomStartRange,
                                                  float startControlPointRange,
                                                  Vector3 endPosition,
                                                  float randomEndRange,
                                                  float endControlPointRange,
                                                  float eachBurstAnimTime,
                                                  float burstInterval,
                                                  Action onComplete = null,
                                                  Action onEachItemMoveComplete = null)
        {
            if (!isAnimationComplete)
            {
                Complete();
            }

            isAnimationComplete = false;
            onAnimationComplete = onComplete;
            var waitBurstInterval = new WaitForSeconds(burstInterval);
            for (var i = 0; i < spawnCount; i++)
            {
                var randomStartPositionOffset = Random.insideUnitCircle * randomStartRange;
                var randomStartPosition = startPosition +
                                          new Vector3(randomStartPositionOffset.x, randomStartPositionOffset.y, 0f);
                var randomStartControlPoint = randomStartPosition + Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) *
                    Vector3.up * startControlPointRange;

                var randomEndPositionOffset = Random.insideUnitCircle * randomEndRange;
                var randomEndPosition =
                    endPosition + new Vector3(randomEndPositionOffset.x, randomEndPositionOffset.y, 0f);
                var randomEndControlPoint = randomEndPosition +
                                            Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) * Vector3.up *
                                            endControlPointRange;

                var itemTrans = itemPool.Spawn();
                itemTrans.position = randomStartPosition;
                itemTrans.DOPath(new[] {randomEndPosition, randomStartControlPoint, randomEndControlPoint},
                             eachBurstAnimTime,
                             PathType.CubicBezier)
                         .SetEase(Ease.InOutCubic)
                         .onComplete = () =>
                {
                    itemPool.Release(itemTrans);
                    onEachItemMoveComplete?.Invoke();
                };
                if (i < spawnCount - 1)
                {
                    yield return waitBurstInterval;
                }
            }

            yield return new WaitForSeconds(eachBurstAnimTime);

            isAnimationComplete = true;
            onComplete?.Invoke();

            cor = null;
        }

        public void Complete(bool callCompleteCallback = false)
        {
            isAnimationComplete = true;
            cor.StopCoroutine();
            cor = null;

            itemPool.ReleaseAllObtainedObjects();

            if (callCompleteCallback)
            {
                onAnimationComplete?.Invoke();
            }

            onAnimationComplete = null;
        }

        public void Dispose()
        {
            Complete();

            itemPool.Dispose();
        }
    }
}
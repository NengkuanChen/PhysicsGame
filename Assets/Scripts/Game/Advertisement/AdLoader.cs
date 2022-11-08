using System;
using UnityEngine;

namespace Game.Advertisement
{
    public class AdLoader
    {
        private Action<int> loadFunction;
        private float nextLoadCountDown;
        private int retryTimes;
        private bool needToLoad;

        public AdLoader(Action<int> loadFunction)
        {
            this.loadFunction = loadFunction;
        }

        public void TryLoad()
        {
            nextLoadCountDown = Mathf.Pow(2, retryTimes + 1);
            needToLoad = true;
        }

        public void Update(float deltaTime)
        {
            if (nextLoadCountDown <= 0f)
            {
                if (needToLoad)
                {
                    nextLoadCountDown = 0f;
                    needToLoad = false;
                    loadFunction?.Invoke(retryTimes);
                }
            }
            else
            {
                nextLoadCountDown -= deltaTime;
            }
        }

        public void OnLoadSuccess()
        {
            retryTimes = 0;
            nextLoadCountDown = 0f;
        }

        public void OnLoadFailure()
        {
            retryTimes++;
        }
    }
}
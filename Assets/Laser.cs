using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Chromatose
{
    public class Laser : MonoBehaviour
    {
        private LineRenderer line;
        float startWidth = 0;
        float endWidth = .4f;
        float fireDuration = .4f;

        void Start()
        {
            line = GetComponentInChildren<LineRenderer>();
            line.enabled = false;
            InvokeRepeating("FireLaser", 2f, 2f);
        }

        public void FireLaser()
        {
            line.enabled = true;
            Timing.RunCoroutine(C_FireLaser(startWidth, endWidth, RepeatMode.OnceAndBack));
        }

        private IEnumerator<float> C_FireLaser(float startWidth, float endWidth, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            float width;
            while (timer <= fireDuration)
            {
                timer = Time.time - startTime;
                width = Mathf.Lerp(startWidth, endWidth, timer / fireDuration);
                line.startWidth = width;
                line.endWidth = width;
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_FireLaser(endWidth, startWidth, RepeatMode.Once), tag);
					break;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Chromatose
{
    public class LightAnimate : MonoBehaviour
    {
        private Light _light;
        private static int tagCounter = 0;
        private string animTag;

        public void Awake()
        {
            _light = GetComponent<Light>();

            // Hacky fix so KillCoroutines won't stop ALL Timing coroutines.
            animTag = name + tagCounter + "";
            tagCounter++;
        }

        public void AnimateToColor(Color finish, float t, RepeatMode mode)
        {
            Timing.KillCoroutines(animTag + "color");
            Timing.RunCoroutine(C_AnimateToColor(_light.color, finish, t, mode), animTag + "color");
        }

        private IEnumerator<float> C_AnimateToColor(Color start, Color finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration)
            {
                timer = Time.time - startTime;
                _light.color = Color.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToColor(finish, start, duration, RepeatMode.Once), animTag);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToColor(finish, start, duration, RepeatMode.PingPong), animTag);
                    break;
                default:
                    break;
            }
        }

        public void AnimateToIntensity(float finish, float t, RepeatMode mode)
        {
            Timing.KillCoroutines(animTag + "intensity");
            Timing.RunCoroutine(C_AnimateToIntensity(_light.intensity, finish, t, mode), animTag + "intensity");
        }

        private IEnumerator<float> C_AnimateToIntensity(float start, float finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration)
            {
                timer = Time.time - startTime;
                _light.intensity = Mathf.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToIntensity(finish, start, duration, RepeatMode.Once), animTag);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToIntensity(finish, start, duration, RepeatMode.PingPong), animTag);
                    break;
                default:
                    break;
            }
        }

        public void AnimateToRange(float finish, float t, RepeatMode mode)
        {
            Timing.KillCoroutines(animTag + "range");
            Timing.RunCoroutine(C_AnimateToRange(_light.range, finish, t, mode), animTag + "range");
        }

        private IEnumerator<float> C_AnimateToRange(float start, float finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration)
            {
                timer = Time.time - startTime;
                _light.range = Mathf.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToRange(finish, start, duration, RepeatMode.Once), animTag);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToRange(finish, start, duration, RepeatMode.PingPong), animTag);
                    break;
                default:
                    break;
            }
        }

        public void Die() {
            AnimateToColor(Color.black, Level.secondsPerMeasure, RepeatMode.Once);
            Invoke("DestroySelf", Level.secondsPerMeasure);
        }

        private void DestroySelf() {
            Destroy(this.gameObject);
        }

        public void StopAnimating()
        {
            Timing.KillCoroutines(tag);
        }

        void OnDestroy()
        {
            Timing.KillCoroutines(tag);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Chromatose
{
    public enum RepeatMode
    {
        Once, OnceAndBack, PingPong
    }

    public class Animate : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private static int tagCounter = 0;
        private string tag;

        public void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Hacky fix so KillCoroutines won't stop ALL Timing coroutines.
            tag = tagCounter + "";
            tagCounter++;
        }

        // POSITION

        public void AnimateToPosition(Vector3 start, Vector3 finish, float t, RepeatMode mode)
        {
            Timing.RunCoroutine(C_AnimateToPosition(start, finish, t, mode), tag);
        }

        private IEnumerator<float> C_AnimateToPosition(Vector3 start, Vector3 finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration)
            {
                timer = Time.time - startTime;
                transform.position = Vector3.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToPosition(finish, start, duration, RepeatMode.Once), tag);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToPosition(finish, start, duration, RepeatMode.PingPong), tag);
                    break;
                default:
                    break;
            }
        }

        // SIZE

        public void AnimateToSize(Vector2 start, Vector2 finish, float t, RepeatMode mode)
        {
            Timing.RunCoroutine(C_AnimateToSize(start, finish, t, mode), tag);
        }

        private IEnumerator<float> C_AnimateToSize(Vector2 start, Vector2 finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration)
            {
                timer = Time.time - startTime;
                transform.localScale = Vector2.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToSize(finish, start, duration, RepeatMode.Once), tag);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToSize(finish, start, duration, RepeatMode.PingPong), tag);
                    break;
                default:
                    break;
            }
        }

        // ROTATION

        public void AnimateToRotation(Quaternion start, Quaternion finish, float t, RepeatMode mode)
        {
            Timing.RunCoroutine(C_AnimateToRotation(start, finish, t, mode), tag);
        }

        private IEnumerator<float> C_AnimateToRotation(Quaternion start, Quaternion finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration)
            {
                timer = Time.time - startTime;
                transform.localRotation = Quaternion.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToRotation(finish, start, duration, RepeatMode.Once), tag);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToRotation(finish, start, duration, RepeatMode.PingPong), tag);
                    break;
                default:
                    break;
            }
        }

        // COLOR

        public void AnimateToColor(Color start, Color finish, float t, RepeatMode mode)
        {
            Timing.RunCoroutine(C_AnimateToColor(start, finish, t, mode), tag);
        }

        private IEnumerator<float> C_AnimateToColor(Color start, Color finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration)
            {
                timer = Time.time - startTime;
                spriteRenderer.color = Color.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToColor(finish, start, duration, RepeatMode.Once), tag);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToColor(finish, start, duration, RepeatMode.PingPong), tag);
                    break;
                default:
                    break;
            }
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
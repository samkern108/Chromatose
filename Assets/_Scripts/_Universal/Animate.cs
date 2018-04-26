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
        private string animTag;

        public enum AnimType
        {
            Color, Size, Position, Rotation
        }

        public void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Hacky fix so KillCoroutines won't stop ALL Timing coroutines.
            animTag = name + tagCounter;
            tagCounter++;
        }

        // POSITION

        public void AnimateToPosition(Vector3 start, Vector3 finish, float t, RepeatMode mode)
        {
            Timing.KillCoroutines(animTag + AnimType.Position);
            Timing.RunCoroutine(C_AnimateToPosition(start, finish, t, mode), animTag + AnimType.Position);
            animPositionCounter++;
        }

        private int animPositionCounter = 0;
        private IEnumerator<float> C_AnimateToPosition(Vector3 start, Vector3 finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration && transform != null)
            {
                timer = Time.time - startTime;
                transform.position = Vector3.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToPosition(finish, start, duration, RepeatMode.Once), animTag + AnimType.Position);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToPosition(finish, start, duration, RepeatMode.PingPong), animTag + AnimType.Position);
                    break;
                default:
                    break;
            }
        }

        // SIZE

        public void AnimateToSize(Vector2 start, Vector2 finish, float t, RepeatMode mode)
        {
            Timing.KillCoroutines(animTag + AnimType.Size);
            Timing.RunCoroutine(C_AnimateToSize(start, finish, t, mode), animTag + AnimType.Size);
        }

        private IEnumerator<float> C_AnimateToSize(Vector2 start, Vector2 finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration && transform != null)
            {
                timer = Time.time - startTime;
                transform.localScale = Vector2.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToSize(finish, start, duration, RepeatMode.Once), animTag + AnimType.Size);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToSize(finish, start, duration, RepeatMode.PingPong), animTag + AnimType.Size);
                    break;
                default:
                    break;
            }
        }

        // ROTATION

        public void AnimateToRotation(Quaternion start, Quaternion finish, float t, RepeatMode mode)
        {
            Timing.KillCoroutines(animTag + AnimType.Rotation);
            Timing.RunCoroutine(C_AnimateToRotation(start, finish, t, mode), animTag + AnimType.Rotation);
        }

        private IEnumerator<float> C_AnimateToRotation(Quaternion start, Quaternion finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration && transform != null)
            {
                timer = Time.time - startTime;
                transform.localRotation = Quaternion.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToRotation(finish, start, duration, RepeatMode.Once), animTag + AnimType.Rotation);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToRotation(finish, start, duration, RepeatMode.PingPong), animTag + AnimType.Rotation);
                    break;
                default:
                    break;
            }
        }

        // COLOR

        public void AnimateToColor(Color finish, float t, RepeatMode mode)
        {
            Timing.KillCoroutines(animTag + AnimType.Color);
            if (spriteRenderer != null)
                Timing.RunCoroutine(C_AnimateToColor(spriteRenderer.color, finish, t, mode), animTag + AnimType.Color);
        }

        public void AnimateToColor(Color start, Color finish, float t, RepeatMode mode)
        {
            Timing.KillCoroutines(animTag + AnimType.Color);
            Timing.RunCoroutine(C_AnimateToColor(start, finish, t, mode), animTag + AnimType.Color);
        }

        private IEnumerator<float> C_AnimateToColor(Color start, Color finish, float duration, RepeatMode mode)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= duration && spriteRenderer != null)
            {
                timer = Time.time - startTime;
                spriteRenderer.color = Color.Lerp(start, finish, timer / duration);
                yield return 0;
            }
            switch (mode)
            {
                case RepeatMode.OnceAndBack:
                    Timing.RunCoroutine(C_AnimateToColor(finish, start, duration, RepeatMode.Once), animTag + AnimType.Color);
                    break;
                case RepeatMode.PingPong:
                    Timing.RunCoroutine(C_AnimateToColor(finish, start, duration, RepeatMode.PingPong), animTag + AnimType.Color);
                    break;
                default:
                    break;
            }
        }

        public void StopAllAnimations()
        {
            Timing.KillCoroutines(animTag + AnimType.Color);
            Timing.KillCoroutines(animTag + AnimType.Position);
            Timing.KillCoroutines(animTag + AnimType.Size);
            Timing.KillCoroutines(animTag + AnimType.Rotation);
        }

        public void StopAnimating(AnimType type)
        {
            Timing.KillCoroutines(animTag + type);
        }

        void OnDestroy()
        {
            StopAllAnimations();
        }
    }
}
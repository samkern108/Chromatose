using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Chromatose
{
    public class LevelMusic : MonoBehaviour
    {
        public float startPitch = 1.0f;
        public static AudioSource audioSource;

        private static LevelMusic self;

        void Awake()
        {
            self = this;
            audioSource = GetComponent<AudioSource>();
            audioSource.pitch = startPitch;
        }

        public static void SetTime(float newTime)
        {
            audioSource.time = newTime;
        }

        public static void Stop()
        {
            audioSource.Stop();
        }

        public static void Play()
        {
            audioSource.Play();
            self.ChangeVolumeInternal(0.0f, 1.0f, Level.secondsPerMeasure);
        }

        public static void ChangeVolume(float startVolume, float endVolume, float time)
        {
            self.ChangeVolumeInternal(startVolume, endVolume, time);
        }

        protected void ChangeVolumeInternal(float startVolume, float endVolume, float time)
        {
            Timing.RunCoroutine(C_ChangeVolume(startVolume, endVolume, time));
        }

        private IEnumerator<float> C_ChangeVolume(float startVolume, float endVolume, float time)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= time)
            {
                timer = Time.time - startTime;
                audioSource.volume = Mathf.Lerp(startVolume, endVolume, timer / time);
                yield return 0;
            }
        }

        public static void ChangePitch(float endPitch, float time)
        {
            self.ChangePitchInternal(endPitch, time);
        }

        protected void ChangePitchInternal(float endPitch, float time)
        {
            Timing.RunCoroutine(C_ChangePitch(audioSource.pitch, endPitch, time));
        }

        private IEnumerator<float> C_ChangePitch(float startPitch, float endPitch, float time)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= time)
            {
                timer = Time.time - startTime;
                audioSource.pitch = Mathf.Lerp(startPitch, endPitch, timer / time);
                yield return 0;
            }
        }
    }
}
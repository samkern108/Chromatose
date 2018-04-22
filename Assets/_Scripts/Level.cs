using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using MEC;
using UnityEngine.SceneManagement;

namespace Chromatose
{
    public class Level : MonoBehaviour
    {
        [EventID]
        public string levelEventID;

        [EventID]
        public string speedEventID;

        public static Koreography koreo;
        private static KoreographyTrackBase track;

        private static int stageNumber = -1;

        public GameObject[] stages;
        public static bool moveToNextStage = false;
        public static float BPM;

        public static float secondsPerBeat, secondsPerMeasure;

        public Palette.LevelColor level;
        public static LevelPalette levelPalette;

        public static AudioSource audioSource;

        public static Level self;

        void Awake()
        {
            Palette.InitColors();

            levelPalette = Palette.levels[level];
            self = this;

            foreach (GameObject stage in stages)
            {
                stage.SetActive(false);
            }
            stages[0].SetActive(true);

            koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
            track = koreo.GetTrackByID(levelEventID);
            track = koreo.GetTrackByID(speedEventID);
            BPM = (float)koreo.GetBPM(0);
            Koreographer.Instance.RegisterForEventsWithTime(levelEventID, LoopEvent);
            Koreographer.Instance.RegisterForEventsWithTime(speedEventID, ChangeSpeedEvent);

            audioSource = GetComponent<AudioSource>();
            secondsPerBeat = 60.0f / BPM;
            secondsPerMeasure = secondsPerBeat * 4.0f;

            LoadNextStage();
            audioSource.Play();
            Timing.RunCoroutine(C_ChangeVolume(0.0f, 1.0f, secondsPerMeasure));
        }

        void OnDestroy()
        {
            if (Koreographer.Instance != null)
                Koreographer.Instance.UnregisterForAllEvents(this);
        }

        public static KoreographyEvent loopStart;

        void LoopEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            int payload = ((IntPayload)evt.Payload).IntVal;
            if (payload == 2)
            {
                loopStart = evt;
            }
            if (payload == 3)
            {
                if (!moveToNextStage)
                {
                    Timing.RunCoroutine(C_ChangeVolume(1.0f, 0.0f, secondsPerBeat));
                    Invoke("LoopMusic", secondsPerBeat);
                }
                else
                {
                    LoadNextStage();
                    moveToNextStage = false;
                }
            }
            //Debug.Log("LoopEvent " + stageNumber + "  " + loopStart.StartSample);
        }

        private void LoopMusic()
        {
            Debug.Log("Loop Music!");
            // start the loop over from the beginning
            audioSource.Stop();
            Koreographer.Instance.FlushDelayQueue(koreo);
            koreo.ResetTimings();
            float newTime = (float)loopStart.StartSample / (float)koreo.SampleRate;
            audioSource.time = newTime;
            audioSource.Play();
            Timing.RunCoroutine(C_ChangeVolume(0.0f, 1.0f, secondsPerBeat));
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

        float initialPitch = 0;

        public void ChangeSpeedEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            float payload = ((FloatPayload)evt.Payload).FloatVal;
            if (initialPitch == 0)
            {
                initialPitch = payload;
                audioSource.pitch = initialPitch;
            }
            else
            {
                Debug.Log("Change Speed Event " + moveToNextStage);
                if (moveToNextStage)
                    Timing.RunCoroutine(C_ChangePitch(audioSource.pitch, payload, secondsPerMeasure));
            }
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

        private void LoadNextStage()
        {
            Debug.Log("Load Next Stage " + stageNumber);
            if (stageNumber >= 0)
            {
                stages[stageNumber].SetActive(false);
            }
            stageNumber++;
            if (stageNumber < stages.Length)
            {
                Debug.Log("Stage number set active now");
                stages[stageNumber].SetActive(true);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        public void PlayerRespawn()
        {
            int latestSample = koreo.GetLatestSampleTime();
            float samplesPerBeat = (float)koreo.GetSamplesPerBeat(latestSample);
            float lastBeatSample = Mathf.Floor((float)(latestSample / samplesPerBeat)) * samplesPerBeat;
            float beatOffset = (samplesPerBeat + lastBeatSample) - koreo.GetLatestSampleTime();
            beatOffset /= koreo.SampleRate;

            Invoke("RespawnOnBeat", beatOffset + secondsPerMeasure);
        }

        private void RespawnOnBeat()
        {
            PlayerController.Respawn();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using UnityEngine.SceneManagement;

namespace Chromatose
{
    public class Level : MonoBehaviour
    {
        [EventID]
        public string eventID;

        public static Koreography koreo;
        private static KoreographyTrackBase track;

        private static int stageNumber = -1;

        public GameObject[] stages;
        public static bool moveToNextStage = true;
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

            koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
            track = koreo.GetTrackByID(eventID);
            BPM = (float)koreo.GetBPM(0);
            Koreographer.Instance.RegisterForEventsWithTime(eventID, LoopEvent);

            audioSource = GetComponent<AudioSource>();
            secondsPerBeat = 60.0f / 126.0f;
            secondsPerMeasure = secondsPerBeat * 4.0f;

            Invoke("StartMusic", 2.0f);
        }

        private void StartMusic() {
            audioSource.Play();
        }

        public static KoreographyEvent loopStart;

        void LoopEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            if (moveToNextStage)
            {
                loopStart = evt;
                LoadNextStage();
                moveToNextStage = false;
            }
            else
            {
                audioSource.Stop();
                Koreographer.Instance.FlushDelayQueue(koreo);
			    koreo.ResetTimings();
                float newTime = (float)loopStart.StartSample / (float)koreo.SampleRate;
                audioSource.time = newTime;
                audioSource.Play();
            }
            Debug.Log("LoopEvent " + stageNumber + "  " + loopStart.StartSample);
        }

        private void LoadNextStage()
        {
            Debug.Log("Load Next Stage");
            if (stageNumber >= 0)
            {
                stages[stageNumber].SetActive(false);
            }
            stageNumber++;
            if (stageNumber < stages.Length)
            {
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
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

        [HideInInspector]
        public static float BPM = 126;

        public static float secondsPerBeat, secondsPerMeasure;
        public static int levelID;
        public LevelColors levelColor;

        public static AudioSource audioSource;

        public static Level self;

        void Awake()
        {
            levelID = (int)levelColor;
            self = this;

            foreach (GameObject stage in stages)
            {
                stage.SetActive(false);
            }

            Palette.InitColors();

            koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
            track = koreo.GetTrackByID(eventID);
            BPM = (float)koreo.GetBPM(0);
            Koreographer.Instance.RegisterForEventsWithTime(eventID, LoopEvent);

            audioSource = GetComponent<AudioSource>();
            secondsPerBeat = 60.0f / 126.0f;
            secondsPerMeasure = secondsPerBeat * 4.0f;
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
                float newTime = (float)loopStart.StartSample / (float)koreo.SampleRate;
                audioSource.time = newTime;
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
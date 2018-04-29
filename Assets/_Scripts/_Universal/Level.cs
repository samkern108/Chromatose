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
        public static Koreography koreo;
        private static KoreographyTrackBase track;
        public static float BPM;

        public static float secondsPerBeat, secondsPerMeasure;

        public Palette.LevelColor level;
        public static LevelPalette levelPalette;

        public static Level self;

        public static bool stopLooping = false;

        public Canvas winCanvas;
        public GameObject winFireworks;

        public static bool gameEnded = false;

        void Awake()
        {
            Palette.InitColors();

            levelPalette = Palette.levels[level];
            self = this;

            koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
            track = koreo.GetTrackByID(levelEventID);
            BPM = (float)koreo.GetBPM(0);
            Koreographer.Instance.RegisterForEventsWithTime(levelEventID, LoopEvent);

            secondsPerBeat = 60.0f / BPM;
            secondsPerMeasure = secondsPerBeat * 4.0f;
        }

        public void StartGameDelayed()
        {
            Invoke("LoadStartStage", secondsPerMeasure);
            Invoke("StartGame", secondsPerMeasure * 2.0f);
        }

        public void LoadStartStage()
        {
            StageManager.self.LoadStartStage();
            LevelMusic.Play();
        }

        public void StartGame()
        {
            StageManager.self.NextStageMusicStart();
            PlayerRespawn();
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
                loopStart = evt;
            if (payload == 3)
            {
                if (!stopLooping)
                {
                    LevelMusic.ChangeVolume(1.0f, 0.0f, secondsPerBeat);
                    Invoke("LoopMusic", secondsPerBeat);
                }
                else
                {
                    stopLooping = false;
                    Debug.Log("Start Music: " + sampleTime);
                    StageManager.self.NextStageMusicStart();
                }
            }
        }

        private void LoopMusic()
        {
            Debug.Log("Loop Music: " + koreo.GetLatestSampleTime());
            // start the loop over from the beginning
            LevelMusic.Stop();
            Koreographer.Instance.FlushDelayQueue(koreo);
            koreo.ResetTimings();
            float newTime = (float)loopStart.StartSample / (float)koreo.SampleRate;
            LevelMusic.SetTime(newTime);
            LevelMusic.Play();
        }

        public void LevelEnded()
        {
            winCanvas.gameObject.SetActive(true);
            winFireworks.SetActive(true);
            stopLooping = true;
            gameEnded = true;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using MEC;

namespace Chromatose
{
    // Payloads:
    // 0 is 1 beat flash to light
    // 1 is 2 beat flash to light
    // 2 is 3 beat flash to light
    // 3 is 4 beat flash to light
    // 5 is 1 beat flash to ultra
    // and so on

    public class BackgroundColor : MonoBehaviour, INotifyOnHitObserver
    {
        public static Bounds bounds;

        [EventID]
        public string eventID;
        public float lastColorChangeFrame;
        private string tag = "background";
        private SpriteRenderer spriteRenderer;

        public static Color baseColor, lightColor, ultraColor;

        private Koreography koreo;
        private KoreographyTrackBase track;

        private int kIndex = 0;

        private float flashDuration = .5f;

        private List<KoreographyEvent> kEvents;

        public static BackgroundColor self;

        void Start()
        {
            self = this;

            //Koreographer.Instance.RegisterForEventsWithTime(eventID, CameraColorEvent);
            spriteRenderer = GetComponent<SpriteRenderer>();
            bounds = spriteRenderer.bounds;

            baseColor = StageConstants.self.backgroundBase;
            lightColor = StageConstants.self.backgroundLight;
            ultraColor = StageConstants.self.backgroundUltra;

            koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
            track = koreo.GetTrackByID(eventID);
            kEvents = track.GetAllEvents();
        }

        void OnDestroy()
        {
            if (Koreographer.Instance != null)
                Koreographer.Instance.UnregisterForAllEvents(this);
        }

        void CameraColorEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            /*int payload = ((IntPayload)evt.Payload).IntVal;
            int colorValue = Mathf.FloorToInt(payload / 4);
            int numBeats = payload % 4;
            Color newColor = Level.levelPalette.darkColor; //(payload == 0) ? lightColor : ultraColor;

            float decayTime = Level.secondsPerBeat * (numBeats + 1);
            //Debug.Log(Level.secondsPerBeat + "   " + numBeats);
            Timing.RunCoroutine(C_AnimateToColor(baseColor, newColor, flashDuration, decayTime), tag);*/

            if ((kIndex + 1) < kEvents.Count && Time.frameCount != lastColorChangeFrame)
            {
                Timing.KillCoroutines(tag);
                int payload = ((IntPayload)evt.Payload).IntVal;
                Color newColor = (payload == 0) ? lightColor : ultraColor;

                float decayTime = (1 / LevelMusic.audioSource.pitch) * (kEvents[kIndex + 1].StartSample - kEvents[kIndex].StartSample) / 40000;
                Timing.RunCoroutine(C_AnimateToColor(baseColor, newColor, (1 / LevelMusic.audioSource.pitch) * .05f, decayTime), tag);
                kIndex++;

                lastColorChangeFrame = Time.frameCount;

                if (Koreographer.Instance != null && (kIndex + 1) >= kEvents.Count)
                    Koreographer.Instance.UnregisterForAllEvents(this);
            }

        }

        private Color currentColor;
        private IEnumerator<float> C_AnimateToColor(Color start, Color finish, float inDuration, float outDuration)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= inDuration)
            {
                timer = Time.time - startTime;
                spriteRenderer.color = Color.Lerp(start, finish, timer / inDuration);
                currentColor = spriteRenderer.color;
                yield return 0;
            }

            startTime = Time.time;
            timer = 0;
            while (timer <= outDuration)
            {
                timer = Time.time - startTime;
                spriteRenderer.color = Color.Lerp(finish, baseColor, timer / outDuration);
                currentColor = spriteRenderer.color;
                yield return 0;
            }
        }

        public static float roomSidesBuffer = 6.0f / 7.0f;
        public static Vector3 GetRandomPointInRoom()
        {
            // roomSidesBuffer prevents it from spawning on the absolute limits of the box.
            float x = bounds.extents.x * roomSidesBuffer;
            float y = bounds.extents.y * roomSidesBuffer;
            Vector3 center = bounds.center;

            Vector3 point = Vector3.zero;
            point.x = Random.Range(center.x - x, center.x + x);
            point.y = Random.Range(center.y - y, center.y + y);

            return point;
        }

        public void NotifyOnHit(EnemyHealth health, bool dead)
        {
            Timing.KillCoroutines(tag);
            Color newColor = (dead) ? ultraColor : lightColor;
            float decayTime = Level.secondsPerBeat * (1 / LevelMusic.audioSource.pitch);
            decayTime *= (dead) ? 4.0f : 2.0f;

            Timing.RunCoroutine(C_AnimateToColor(baseColor, newColor, (1 / LevelMusic.audioSource.pitch) * .05f, decayTime), tag);
        }
    }
}
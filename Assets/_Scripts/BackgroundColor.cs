using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using MEC;

namespace Chromatose
{
    public class BackgroundColor : MonoBehaviour
    {
		public static Bounds bounds;

        [EventID]
        public string eventID;
        public float lastColorChangeFrame;
        private string tag = "background";
        private SpriteRenderer spriteRenderer;

        private Color baseColor, lightColor, ultraColor;

        private Koreography koreo;
        private KoreographyTrackBase track;
        private List<KoreographyEvent> kEvents;
        private int kIndex = 0;

        void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(eventID, CameraColorEvent);
            spriteRenderer = GetComponent<SpriteRenderer>();
			bounds = spriteRenderer.bounds;

            baseColor = Palette.levelColors[Level.levelID];
            lightColor = Palette.levelColorsLight[Level.levelID];
            ultraColor = Palette.levelColorsUltra[Level.levelID];

            koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
            track = koreo.GetTrackByID(eventID);
            kEvents = track.GetAllEvents();
        }

        void OnDestroy()
        {
            if (Koreographer.Instance != null)
            {
                Koreographer.Instance.UnregisterForAllEvents(this);
            }
        }

        void CameraColorEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            if (Time.frameCount != lastColorChangeFrame)
            {
                Timing.KillCoroutines(tag);
                int payload = 0;//((IntPayload)evt.Payload).IntVal;
                Color newColor = (payload == 0) ? lightColor : ultraColor;

                float decayTime = (kEvents[kIndex + 1].StartSample - kEvents[kIndex].StartSample) / 40000;
                Timing.RunCoroutine(C_AnimateToColor(baseColor, newColor, .05f, decayTime), tag);
                kIndex++;

                lastColorChangeFrame = Time.frameCount;
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
    }
}
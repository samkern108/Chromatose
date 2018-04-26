using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using System.Linq;

namespace Chromatose
{
    public interface IBlinkObserver
    {
        void Blink();
    }

    public class BlinkTrack : MonoBehaviour
    {
        public IBlinkObserver blinkObserver;
        public GameObject blinkInEffect;
        public float blinkInLengthMod = 4f;

        [EventID]
        public string eventID;
        private Koreography koreo;
        public Track track;

        private bool canMove = false;

        void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(eventID, BlinkEvent);

            blinkObserver = GetComponentInChildren<IBlinkObserver>();

            koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        }

        void OnDestroy()
        {
            if (Koreographer.Instance != null)
                Koreographer.Instance.UnregisterForAllEvents(this);
        }

        private Vector3 blinkPosition;

        void BlinkEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            if (canMove)
            {
                Invoke("Blink", blinkInLengthMod * Level.secondsPerBeat);
                blinkPosition = track.GetRandomPointExcludingCurrent().position;

                BlinkInEffect();
            }
        }

        private void BlinkInEffect()
        {
            Vector3 newPosition = blinkPosition;
            newPosition.z = 0;
            blinkInEffect.transform.position = newPosition;

            blinkInEffect.SetActive(true);
        }

        private void Blink()
        {
            blinkInEffect.SetActive(false);
            transform.position = blinkPosition;
            blinkObserver.Blink();
        }

        public void LoadingStage()
        {
            canMove = false;
        }

        public void StageBegin()
        {
            canMove = true;
        }
    }
}
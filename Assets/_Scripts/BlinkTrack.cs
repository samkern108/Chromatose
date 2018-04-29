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

        private Turret turret;

        private bool canMove = false;

        private EnemyHealth childHealth;

        void Start()
        {
            turret = GetComponentInChildren<Turret>();
            childHealth = GetComponentInChildren<EnemyHealth>();

            Koreographer.Instance.RegisterForEventsWithTime(eventID, BlinkEvent);

            blinkObserver = GetComponentInChildren<IBlinkObserver>();

            koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        }

        void OnDestroy()
        {
            if (Koreographer.Instance != null)
                Koreographer.Instance.UnregisterForAllEvents(this);
            Destroy(blinkInEffect);
        }

        private Vector3 blinkPosition;
        private bool prewarmed = false;

        void BlinkEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            if(childHealth == null) {
                blinkInEffect.SetActive(false);
                Destroy(this);
            }

            if (canMove)
            {
                if (!prewarmed)
                {
                    blinkPosition = track.GetRandomPointExcludingCurrent().position;
                    BlinkInEffect();
                }
                else
                {
                    Blink();
                    if(turret.enabled)
                        turret.ShootMissile();
                }
                prewarmed = !prewarmed;
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
            Invoke("StageBegin", Level.secondsPerMeasure * 2.0f);
        }

        public void StageBegin()
        {
            canMove = true;
        }
    }
}
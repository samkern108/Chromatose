using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using SonicBloom.Koreo;

namespace Chromatose
{
    public class DeathZone : MonoBehaviour
    {
		private SpriteRenderer spriteRenderer;
        Vector3 startScale, endScale;
        float fireDuration = .4f;

        bool firing = true;

        [EventID]
        public string eventID;
        private Koreography koreo;

        void Start()
        {
			spriteRenderer = GetComponent<SpriteRenderer>();
            Koreographer.Instance.RegisterForEventsWithTime(eventID, WarmUpEvent);
            fireDuration = Level.secondsPerBeat;
            startScale = transform.localScale / 2.0f;
			endScale = transform.localScale;
			transform.localScale = startScale;
        }

        void WarmUpEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            Timing.RunCoroutine(C_WarmUpZone(startScale, 3.0f * endScale / 4.0f, BackgroundColor.baseColor, BackgroundColor.ultraColor, true));
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (firing)
            {
				col.gameObject.SendMessage("Die");
            }
        }

		public void OnCollisionEnter2D(Collision2D col)
        {
            if (firing)
            {
				col.gameObject.SendMessage("Die");
            }
        }

        public void FireZone()
        {
            Timing.RunCoroutine(C_FireZone());
        }

        private void CoolDownZone()
        {
            Timing.RunCoroutine(C_WarmUpZone(3.0f * endScale / 4.0f, startScale, BackgroundColor.ultraColor, BackgroundColor.baseColor, false));
        }

        private IEnumerator<float> C_WarmUpZone(Vector3 startScale, Vector3 endScale, Color startColor, Color endColor, bool fire)
        {
            float startTime = Time.time;
            float timer = 0;
            Vector3 scale;
			spriteRenderer.color = startColor;
            float fluct = .09f;
            while (timer <= fireDuration)
            {
                timer = Time.time - startTime;
                scale = Vector3.Lerp(startScale, endScale, timer / fireDuration);
				transform.localScale = scale + new Vector3(fluct, fluct, 0);
                spriteRenderer.color = Color.Lerp(startColor, endColor, timer / fireDuration);
                fluct *= -1;
                yield return 0;
            }
			transform.localScale = endScale;
            if (fire)
            {
                FireZone();
            }
        }

        private IEnumerator<float> C_FireZone()
        {
			spriteRenderer.color = Color.white;
            transform.localScale = endScale;
            firing = true;

            float startTime = Time.time;
            float timer = 0;
            float fluct = .2f;
            while (timer <= fireDuration)
            {
                timer = Time.time - startTime;
				transform.localScale += new Vector3(fluct, fluct, 0);
                fluct *= -1;
                yield return 0;
            }
			firing = false;
            CoolDownZone();
        }
    }
}
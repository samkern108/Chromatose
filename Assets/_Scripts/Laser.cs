using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using SonicBloom.Koreo;

namespace Chromatose
{
    public class Laser : MonoBehaviour
    {
        private LineRenderer line;
        float startWidth = 0;
        float endWidth = .4f;
        float fireDuration = .4f;

        bool firing = false;

        [EventID]
        public string eventID;
        private Koreography koreo;

        void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(eventID, WarmUpEvent);
            fireDuration = Level.secondsPerBeat;
            line = GetComponentInChildren<LineRenderer>();
            line.enabled = false;
        }

        void WarmUpEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            line.enabled = true;
            Timing.RunCoroutine(C_WarmUpLaser(startWidth, 3.0f * endWidth / 4.0f, BackgroundColor.baseColor, BackgroundColor.ultraColor));
        }

        public void Update()
        {
            if (firing)
            {
                Vector3 checkPosition = transform.position;
                checkPosition.x -= endWidth / 2.0f;
                RaycastHit2D[] hits = Physics2D.RaycastAll(checkPosition, Vector2.down, 20.0f);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        hit.collider.gameObject.SendMessage("Die");
                    }
                }

                checkPosition.x += endWidth;
                hits = Physics2D.RaycastAll(checkPosition, Vector2.down, 20.0f);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        hit.collider.gameObject.SendMessage("Die");
                    }
                }
            }
        }

        public void FireLaser()
        {
            line.endColor = Color.white;
            line.startColor = Color.white;

            line.startWidth = endWidth;
            line.endWidth = endWidth;

            Timing.RunCoroutine(C_FireLaser());
        }

        private void CoolDownLaser()
        {
            Timing.RunCoroutine(C_WarmUpLaser(3.0f * endWidth / 4.0f, startWidth, BackgroundColor.ultraColor, BackgroundColor.baseColor));
        }

        private IEnumerator<float> C_WarmUpLaser(float startWidth, float endWidth, Color startColor, Color endColor)
        {
            float startTime = Time.time;
            float timer = 0;
            float width;
            line.startColor = startColor;
            line.endColor = startColor;
            float fluct = .07f;
            while (timer <= fireDuration)
            {
                timer = Time.time - startTime;
                width = Mathf.Lerp(startWidth, endWidth, timer / fireDuration);
                line.startWidth = width + fluct;
                line.endWidth = width + fluct;
                line.startColor = Color.Lerp(startColor, endColor, timer / fireDuration);
                line.endColor = line.startColor;
                fluct *= -1;
                yield return 0;
            }
            if (endWidth == this.startWidth)
            {
                line.enabled = false;
            }
            else
            {
                FireLaser();
            }
        }

        private IEnumerator<float> C_FireLaser()
        {
            firing = true;
            float startTime = Time.time;
            float timer = 0;
            float fluct = .04f;
            while (timer <= fireDuration)
            {
                timer = Time.time - startTime;
                line.startWidth = line.startWidth + fluct;
                line.endWidth = line.startWidth + fluct;
                fluct *= -1;
                yield return 0;
            }
            firing = false;
            CoolDownLaser();
        }
    }
}
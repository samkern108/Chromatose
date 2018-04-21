using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

namespace Chromatose
{
    public class Drone : MonoBehaviour
    {

        [EventID]
        public string eventID;
        private Koreography koreo;

        private float projectileSpeed = 7.0f;

        private Vector3 startSize, bigSize;

        private float shootAngle = 45f;

        public GameObject projectile;
        private Animate animate;

        void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(eventID, ShootEvent);
            animate = GetComponent<Animate>();
        }

        void OnDestroy()
        {
            if (Koreographer.Instance != null)
                Koreographer.Instance.UnregisterForAllEvents(this);
        }

        void ShootEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            GameObject missile;
            Vector3 direction;
            float angle = shootAngle;
            for (int i = 0; i < 4; i++)
            {
                missile = Instantiate(projectile);//, ProjectileManager.myTransform);
                missile.transform.position = transform.position;
                direction = Vector3.up.Rotate2D(angle);
                angle += 90;
                missile.GetComponent<Projectile>().Initialize(direction, projectileSpeed);
            }
            shootAngle -= 45;
            startSize = transform.localScale;
            bigSize = startSize + (startSize * .2f);
            animate.AnimateToSize(startSize, bigSize, .2f, RepeatMode.OnceAndBack);
        }
    }
}
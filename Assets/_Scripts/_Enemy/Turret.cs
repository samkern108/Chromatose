using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

namespace Chromatose
{
    public class Turret : MonoBehaviour
    {
        [EventID]
        public string eventID;
        protected Koreography koreo;
        protected Vector3 startSize, bigSize;
        public float shootAngle = 45f;
        public GameObject projectile;
        protected Animate animate;
        public int splitCount = 0;
        public float projectileSpeed = 7.0f;

        public bool turretActive = true;

        public bool skip = false;

        void Start()
        {
            //Koreographer.Instance.RegisterForEventsWithTime(eventID, ShootEvent);
            animate = GetComponent<Animate>();
        }

        void OnDestroy()
        {
            if (Koreographer.Instance != null)
                Koreographer.Instance.UnregisterForAllEvents(this);
        }

        void ShootEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            if(turretActive)
                ShootMissile();
        }

        public virtual void ShootMissile()
        {
            GameObject missile;
            Vector3 direction;
            float angle = shootAngle;
            int shootCounter = (skip) ? 2 : 1;
            for (int i = 0; i < 4; i += shootCounter)
            {
                missile = Instantiate(projectile);//, ProjectileManager.myTransform);
                missile.transform.position = transform.position;
                direction = Vector3.up.Rotate2D(angle);
                angle += 90;
                missile.GetComponent<Projectile>().Initialize(direction, projectileSpeed, splitCount);
            }
            //shootAngle -= 45;
            startSize = transform.localScale;
            bigSize = startSize + (startSize * .2f);
            animate.AnimateToSize(startSize, bigSize, .05f, RepeatMode.OnceAndBack);
            animate.AnimateToColor(Color.yellow, .05f, RepeatMode.OnceAndBack, AnimPriority.Informative);
        }

        public void LoadingStage() {
            turretActive = false;
            Invoke("StageBegin", Level.secondsPerMeasure * 2.0f);
        }

        public void StageBegin() {
            turretActive = true;
        }
    }
}
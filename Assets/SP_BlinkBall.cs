using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class SP_BlinkBall : MonoBehaviour, INotifyOnHitObserver
    {

        private EnemyHealth[] hitListeners;
        private EnemyHealth myHealth;
        private int level = 0;
        private Turret turret;
		private bool rotate = false;

		private static int ballsLeft;

        public void NotifyOnHit(EnemyHealth health, bool dead)
        {
            if (health == myHealth && dead)
            {
				ballsLeft--;
				if(ballsLeft == 0) {
					Level.self.LevelEnded();
				}
                Destroy(GetComponentInChildren<BlinkTrack>());
                //Destroy(GetComponent<LightAnimate>());
                //Destroy(gameObject);
            }
            if (health != myHealth && dead)
            {
                ProgressOneLevel();
            }
        }

        void Start()
        {
			ballsLeft++;
            turret = GetComponentInChildren<Turret>();
            hitListeners = transform.parent.GetComponentsInChildren<EnemyHealth>();
            myHealth = GetComponentInChildren<EnemyHealth>();
            foreach (EnemyHealth health in hitListeners)
            {
                health.notifyOnHitDelegates.Add(this);
            }
        }

        private void ProgressOneLevel()
        {
            if (turret == null) return;

            level++;
            switch (level)
            {
                case 1:
                    turret.projectileSpeed = 3.0f;
                    turret.skip = false;
                    turret.enabled = true;
					if(rotate) turret.shootAngle = 45;
					else turret.shootAngle = 0;
					rotate = !rotate;
                    turret.splitCount = 0;
                    break;
                case 2:
                    turret.projectileSpeed = 5.0f;
					turret.skip = false;
					if(rotate) turret.shootAngle = 45;
					else turret.shootAngle = 0;
					rotate = !rotate;
                    turret.splitCount = 2;
                    break;
                case 3:
                    turret.projectileSpeed = 7.0f;
                    turret.splitCount = 4;
					turret.shootAngle = 45;
                    break;
            }
        }
    }
}
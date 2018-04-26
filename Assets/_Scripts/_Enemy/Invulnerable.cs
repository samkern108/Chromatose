using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Invulnerable : MonoBehaviour, INotifyOnHitObserver
    {
		private Animate animate;
        private EnemyHealth myHealth;
        public float invulnerableTimeBeatMod = 2f;
		public float invulnerableTimeAfterSpawnMod = 2f;
		
        public void Awake()
        {
            myHealth = GetComponent<EnemyHealth>();
			animate = GetComponent<Animate>();
			myHealth.notifyOnHitDelegates.Add(this);
			if(myHealth.hitListener != null) myHealth.hitListener.notifyOnHitDelegates.Add(this);
        }

        public void TurnOnInvulnerable()
        {
            myHealth.invulnerable = true;
            animate.AnimateToColor(StageConstants.self.enemyInvuln1, StageConstants.self.enemyInvuln2, Level.secondsPerBeat * .1f, RepeatMode.PingPong, AnimPriority.Critical);
        }

        public void TurnOffInvulnerable()
        {
            myHealth.invulnerable = false;
            animate.AnimateToColor(StageConstants.self.enemyInvuln1, Color.black, Level.secondsPerBeat, RepeatMode.Once, AnimPriority.Override);
        }

        public void LoadingStage()
        {
            TurnOnInvulnerable();
        }

        public void StageBegin()
        {
            if(GetComponentInParent<BeatTrack>() == null)
			    Invoke("TurnOffInvulnerable",invulnerableTimeAfterSpawnMod * Level.secondsPerBeat);
        }

        public void NotifyOnHit(EnemyHealth health, bool dead)
        {
            if(!dead && health == myHealth) {
				TurnOnInvulnerable();
				if (myHealth.hitListener == null || !myHealth.hitListener.enabled) 
					Invoke("TurnOffInvulnerable", Level.secondsPerBeat * invulnerableTimeBeatMod);
			}
			else if(!dead && health == myHealth.hitListener) {
				TurnOffInvulnerable();
			}
        }
    }
}
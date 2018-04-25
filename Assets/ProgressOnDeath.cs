using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class ProgressOnDeath : MonoBehaviour, INotifyOnDeathObserver
    {
		public bool progressImmediate = false;

        void Start()
        {
			GetComponentInChildren<EnemyHealth>().notifyDelegates.Add(this);
        }

        public void NotifyOnDeath(EnemyHealth health)
        {
            Stage.activeStage.NotifyOnDeath(health, progressImmediate);
        }
    }
}
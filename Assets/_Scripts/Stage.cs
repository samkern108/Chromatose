using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Stage : MonoBehaviour, INotifyOnDeathObserver
    {

        private int waitingOnObjects;
        private bool stageHasPassed = false;

        public void Start()
        {
            EnemyHealth[] notifiers = GetComponentsInChildren<EnemyHealth>();
            foreach (EnemyHealth notifier in notifiers)
            {
                if(notifier.notifyOnDeath)
                    notifier.notifyDelegate = this;
            }
            waitingOnObjects = notifiers.Length;
        }

        public void NotifyOnDeath(bool progressImmediate)
        {
            if (stageHasPassed)
                return;
            if (progressImmediate)
            {
                stageHasPassed = true;
                Debug.Log("Progress Immediate");
                Level.moveToNextStage = true;
            }
            else
            {
                waitingOnObjects--;
                Debug.Log("Waiting on " + waitingOnObjects + " Objects");
                if (waitingOnObjects <= 0)
                {
                    stageHasPassed = true;
                    Level.moveToNextStage = true;
                }
            }
        }
    }
}
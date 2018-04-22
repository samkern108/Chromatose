using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Stage : MonoBehaviour, INotifyOnDeathObserver
    {
        private int waitingOnObjects;
        private bool stageHasPassed = false;

        public GameObject[] keepOnStageChange;

        public void Start()
        {
            EnemyHealth[] notifiers = GetComponentsInChildren<EnemyHealth>();
            foreach (EnemyHealth notifier in notifiers)
            {
                if (notifier.notifyOnDeath)
                    notifier.notifyDelegate = this;
            }
            waitingOnObjects = notifiers.Length;
        }

        public void NotifyOnDeath(EnemyHealth health)
        {
            if (stageHasPassed)
                return;
            if (health.progressImmediate)
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

        public void StageWillChange(Stage newStage)
        {
            foreach (GameObject obj in keepOnStageChange)
            {
                if (obj != null)
                {
                    obj.transform.SetParent(newStage.transform);
                    StagePiece sp = obj.GetComponent<StagePiece>();
                    if (sp) sp.StageWillChange();
                }
            }
        }
    }
}
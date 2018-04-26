using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Stage : MonoBehaviour
    {
        public enum State { NotStarted = 0, Completed, Loading, Active, Ended };
        public State state;

        public float startPitch;

        private List<EnemyHealth> notifiers = new List<EnemyHealth>();

        public GameObject[] keepOnStageChange;
        public static Stage loadingStage, activeStage;
        private static int activeStageNumber = 0;

        public void Awake()
        {
            state = State.NotStarted;
        }

        public void NotifyOnDeath(EnemyHealth health, bool progressImmediate)
        {
            if (state == State.Completed)
                return;
            if (progressImmediate)
            {
                SetState(State.Completed);
                Debug.Log("Progress Immediate");
            }
            else
            {
                notifiers.Remove(health);
                Debug.Log("Waiting on " + notifiers.Count + " Objects");
                if (notifiers.Count <= 0)
                {
                    SetState(State.Completed);
                }
            }
        }

        public void AddNotifier(EnemyHealth health)
        {
            notifiers.Add(health);
        }

        public void SetState(State newState)
        {
            switch (newState)
            {
                case State.NotStarted:
                    gameObject.SetActive(false);
                    break;
                case State.Loading:
                    gameObject.SetActive(true);
                    loadingStage = this;
                    Debug.Log("Loading");
                    LevelMusic.ChangePitch(startPitch, Level.secondsPerMeasure);

                    BroadcastMessage("LoadingStage");
                    PlayerController.self.LoadingStage();
                    break;
                case State.Active:
                    if (activeStage != null) activeStage.SetState(State.Ended);
                    activeStage = this;
                    Debug.Log("Active");
                    BroadcastMessage("StageBegin");
                    break;
                case State.Completed:
                    Debug.Log("Completed");
                    activeStageNumber++;
                    Stage stageToLoad = StageManager.self.stages[activeStageNumber];

                    BroadcastMessage("StageCompleted");
                    foreach (GameObject obj in keepOnStageChange)
                        if (obj.activeInHierarchy)
                            obj.transform.SetParent(stageToLoad.transform);

                    stageToLoad.SetState(State.Loading);
                    Level.stopLooping = true;
                    break;
                case State.Ended:
                    Debug.Log("Ended");
                    gameObject.SetActive(false);
                    break;
            }
            state = newState;
        }
    }
}
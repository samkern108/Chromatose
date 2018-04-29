using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager self;
        public static bool readyForNextStage = false;
        public Stage[] stages;
        public int startStage = 0;

        void Awake()
        {
            self = this;
            foreach (Stage stage in stages)
            {
                stage.SetState(Stage.State.NotStarted);
            }
            //InvokeRepeating("CheckToProgress", 2.0f, 2.0f);
        }

        public void CheckToProgress() {
            if(Stage.activeStage.GetComponentsInChildren<EnemyBall>().Length == 0) {
                Stage.ForceProgressStage();
            }
        }

        public void LoadStartStage()
        {
            stages[startStage].SetState(Stage.State.Loading);
        }

        public void NextStageMusicStart()
        {
            Stage.loadingStage.SetState(Stage.State.Active);
        }
    }
}
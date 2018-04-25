using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Chromatose
{
    public interface IStageProgressObserver
    {
        void LoadingStage();
        void StageHasBegun();
    }

    public interface IPlayerObserver
    {
        void PlayerDied();
    }

    public class NotificationMaster : MonoBehaviour
    {
        public static List<IPlayerObserver> playerObservers = new List<IPlayerObserver>();
        public static List<IStageProgressObserver> stageProgressObservers = new List<IStageProgressObserver>();

        public static void SendPlayerDeathNotification()
        {
            List<IPlayerObserver> toRemove = new List<IPlayerObserver>();
            foreach (IPlayerObserver o in playerObservers)
            {
                if (o.Equals(null))
                    toRemove.Add(o);
                else
                    o.PlayerDied();
            }
            playerObservers = playerObservers.Except(toRemove).ToList();
        }

        public static void SendLoadingStageNotification(Stage newStage)
        {
            List<IStageProgressObserver> toRemove = new List<IStageProgressObserver>();
            foreach (IStageProgressObserver o in stageProgressObservers)
            {
                if (o.Equals(null))
                    toRemove.Add(o);
                else
                    o.LoadingStage();
            }
            stageProgressObservers = stageProgressObservers.Except(toRemove).ToList();
        }

        public static void SendStageHasBegunNotification(Stage newStage)
        {
            Debug.Log("Stage has begun!");
            List<IStageProgressObserver> toRemove = new List<IStageProgressObserver>();
            foreach (IStageProgressObserver o in stageProgressObservers)
            {
                if (o.Equals(null))
                    toRemove.Add(o);
                else
                {
                    o.StageHasBegun();
                }
            }
            stageProgressObservers = stageProgressObservers.Except(toRemove).ToList();
        }
    }
}
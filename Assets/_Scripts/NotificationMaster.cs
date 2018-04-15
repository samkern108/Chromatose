using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Chromatose
{
    public interface IRestartObserver
    {
        void Restart();
    }

    public interface IPlayerObserver
    {
        void PlayerDied();
    }

    public interface IPlayerActionObserver
    {
        void PlayerShoot();
        void PlayerAirborne();
        void PlayerGrounded();
    }

    public class NotificationMaster : MonoBehaviour
    {

        public static List<IRestartObserver> restartObservers = new List<IRestartObserver>();
        public static List<IPlayerObserver> playerObservers = new List<IPlayerObserver>();
        public static List<IPlayerActionObserver> playerActionObservers = new List<IPlayerActionObserver>();

        /* TODO(samkern): Possible simplification? go learn about delegates.
        public delegate void RestartDelegate();
        public static List<RestartDelegate> restartDelegates = new List<RestartDelegate>();
        public static void SendRestartNotification() {
            foreach (RestartDelegate del in restartDelegates) {
                del ();
            }
        }*/

        public static void SendRestartNotification()
        {
            List<IRestartObserver> toRemove = new List<IRestartObserver>();
            foreach (IRestartObserver o in restartObservers)
            {
                if (o.Equals(null))
                    toRemove.Add(o);
                else
                    o.Restart();
            }
            restartObservers = restartObservers.Except(toRemove).ToList();
        }

        /*public static void SendRestartNotification() {
            foreach(IRestartObserver o in restartObservers) {
                o.Restart ();
            }
        }*/

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

        public static void SendPlayerShootNotification()
        {
            List<IPlayerActionObserver> toRemove = new List<IPlayerActionObserver>();
            foreach (IPlayerActionObserver o in playerActionObservers)
            {
                if (o.Equals(null))
                    toRemove.Add(o);
                else
                    o.PlayerShoot();
            }
            playerActionObservers = playerActionObservers.Except(toRemove).ToList();
        }

        public static void SendPlayerAirborneNotification()
        {
            List<IPlayerActionObserver> toRemove = new List<IPlayerActionObserver>();
            foreach (IPlayerActionObserver o in playerActionObservers)
            {
                if (o.Equals(null))
                    toRemove.Add(o);
                else
                    o.PlayerAirborne();
            }
            playerActionObservers = playerActionObservers.Except(toRemove).ToList();
        }

        public static void SendPlayerGroundedNotification()
        {
            List<IPlayerActionObserver> toRemove = new List<IPlayerActionObserver>();
            foreach (IPlayerActionObserver o in playerActionObservers)
            {
                if (o.Equals(null))
                    toRemove.Add(o);
                else
                    o.PlayerGrounded();
            }
            playerActionObservers = playerActionObservers.Except(toRemove).ToList();
        }
    }
}
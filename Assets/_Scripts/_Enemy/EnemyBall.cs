using UnityEngine;
using System.Collections;

namespace Chromatose
{
    public class EnemyBall : MonoBehaviour, IMovementObserver
    {
        private Animate animate;

        void Awake()
        {
            animate = GetComponent<Animate>();
            animate.intendedColor = Color.black;
        }

        public void StartMoving()
        {
            animate.intendedColor = Color.white;
            animate.AnimateToColor(Color.white, Level.secondsPerBeat, RepeatMode.Once, AnimPriority.Informative);
        }

        public void StopMoving()
        {
            animate.intendedColor = Color.black;
            animate.AnimateToColor(Color.black, Level.secondsPerBeat, RepeatMode.Once, AnimPriority.Informative);
        }
    }
}
using UnityEngine;
using System.Collections;

namespace Chromatose
{
    public class EnemyBall : MonoBehaviour, IMovementObserver
    {
        protected Animate animate;

        protected void Awake()
        {
            animate = GetComponent<Animate>();
            animate.intendedColor = Color.black;
        }

        public virtual void StartMoving()
        {
            animate.intendedColor = Color.white;
            animate.AnimateToColor(Color.white, Level.secondsPerBeat, RepeatMode.Once, AnimPriority.Informative);
        }

        public virtual void StopMoving()
        {
            animate.intendedColor = Color.black;
            animate.AnimateToColor(Color.black, Level.secondsPerBeat, RepeatMode.Once, AnimPriority.Informative);
        }
    }
}
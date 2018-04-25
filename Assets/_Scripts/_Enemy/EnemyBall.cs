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
        }

        public void StartMoving()
        {
            animate.AnimateToColor(Color.black, Color.white, Level.secondsPerBeat, RepeatMode.Once);
        }

        public void StopMoving()
        {
            animate.AnimateToColor(Color.white, Color.black, Level.secondsPerBeat, RepeatMode.Once);
        }
    }
}
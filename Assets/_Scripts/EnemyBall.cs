using UnityEngine;
using System.Collections;

namespace Chromatose
{
    public class EnemyBall : MonoBehaviour, IMovementObserver
    {
        private Animate animate;
        private Vector3 startSize, bigSize;

        private void Start()
        {
            animate = GetComponent<Animate>();
            startSize = transform.localScale;
            bigSize = startSize + (startSize * .2f);
            animate.AnimateToSize (Vector3.zero, startSize, Level.secondsPerMeasure, RepeatMode.Once);
        }

        public void StartMoving()
        {
            animate.AnimateToColor(Color.black, Color.white, Level.secondsPerBeat, RepeatMode.Once);
            //animate.AnimateToSize (startSize, bigSize, .2f, Animate.RepeatMode.Once);
        }

        public void StopMoving()
        {
            animate.AnimateToColor(Color.white, Color.black, Level.secondsPerBeat, RepeatMode.Once);
            //animate.AnimateToSize (bigSize, startSize, .2f, Animate.RepeatMode.Once);
        }
    }
}
using UnityEngine;
using System.Collections;

namespace Chromatose
{
    public class EnemyBall : MonoBehaviour
    {
        private Animate animate;
        private Vector3 startSize, bigSize;

        private void Start()
        {
            animate = GetComponent<Animate>();
            startSize = transform.localScale;
            bigSize = startSize + (startSize * .2f);
        }

        public void StartMoving()
        {
            animate.AnimateToColor(Color.black, Color.white, .2f, RepeatMode.Once);
            //animate.AnimateToSize (startSize, bigSize, .2f, Animate.RepeatMode.Once);
        }

        public void StopMoving()
        {
            animate.AnimateToColor(Color.white, Color.black, .2f, RepeatMode.Once);
            //animate.AnimateToSize (bigSize, startSize, .2f, Animate.RepeatMode.Once);
        }
    }
}
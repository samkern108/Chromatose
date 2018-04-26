using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Wall : MonoBehaviour
    {
        private Vector3 normalPosition;

        public void Start()
        {
            normalPosition = transform.position;
			transform.position = (normalPosition - Vector3.zero) / 2.0f;
            //transform.position = Vector3.zero;
        }

        public void AnimateWallOutward()
        {
            Animate animate = GetComponent<Animate>();
            animate.AnimateToPosition(Vector3.zero, normalPosition, Level.secondsPerMeasure, RepeatMode.Once);
        }
    }
}
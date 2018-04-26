using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Chromatose
{
    public class Track : MonoBehaviour
    {
		public Transform[] points;
        private int currentPoint = -1;
        void Awake()
        {
            points = transform.GetComponentsInChildren<Transform>();
            points = points.Skip(1).ToArray();
        }

        public Transform GetRandomPointExcludingCurrent() {
            int newPoint = (int)Random.Range(0, points.Length - 1);
            if(newPoint == currentPoint) newPoint = (newPoint + 1) % points.Length;
            return points[newPoint];
        }
    }
}

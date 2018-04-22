using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Chromatose
{
    public class Track : MonoBehaviour
    {
		public Transform[] points;
        void Awake()
        {
            points = transform.GetComponentsInChildren<Transform>();
            points = points.Skip(1).ToArray();
        }
    }
}
